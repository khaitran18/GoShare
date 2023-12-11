using Application.Common.Dtos;
using Application.Common.Exceptions;
using Application.Common.Utilities;
using Application.Common.Utilities.Google.Firebase;
using Application.Services;
using Application.Services.Interfaces;
using Application.SignalR;
using Application.UseCase.TripUC.Commands;
using AutoMapper;
using Domain.DataModels;
using Domain.Enumerations;
using Domain.Interfaces;
using Hangfire;
using MediatR;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace Application.UseCase.TripUC.Handlers
{
    public class CancelTripHandler : IRequestHandler<CancelTripCommand, TripDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ISettingService _settingService;
        private readonly IMapper _mapper;
        private readonly UserClaims _userClaims;
        private readonly ILogger<BackgroundServices> _logger;
        private readonly IHubContext<SignalRHub> _hubContext;

        public CancelTripHandler(IUnitOfWork unitOfWork, ISettingService settingService, IMapper mapper, UserClaims userClaims, ILogger<BackgroundServices> logger, IHubContext<SignalRHub> hubContext)
        {
            _unitOfWork = unitOfWork;
            _settingService = settingService;
            _mapper = mapper;
            _userClaims = userClaims;
            _logger = logger;
            _hubContext = hubContext;
        }

        public async Task<TripDto> Handle(CancelTripCommand request, CancellationToken cancellationToken)
        {
            var trip = await _unitOfWork.TripRepository.GetByIdAsync(request.TripId);
            if (trip == null)
            {
                throw new NotFoundException(nameof(Trip), request.TripId);
            }

            var booker = await _unitOfWork.UserRepository.GetUserById(trip.BookerId.ToString());
            if (booker == null)
            {
                throw new NotFoundException(nameof(User), trip.BookerId);
            }

            UserRoleEnumerations userRole = _userClaims.Role;
            if (userRole == UserRoleEnumerations.Admin)
            {
                if (trip.Status == TripStatus.PENDING || trip.Status == TripStatus.GOING_TO_PICKUP)
                {
                    throw new BadRequestException("You can only cancel going trip.");
                }

                trip.CanceledBy = null;
            }
            else
            {
                Guid userId = (Guid)_userClaims.id!;

                if (trip.BookerId != userId)
                {
                    throw new BadRequestException("User is not booker of this trip. User can't do this function.");
                }

                if (trip.Status != TripStatus.PENDING && trip.Status != TripStatus.GOING_TO_PICKUP)
                {
                    throw new BadRequestException("You are not allowed to cancel trip at the moment.");
                }

                trip.CanceledBy = userId;
            }

            var now = DateTimeUtilities.GetDateTimeVnNow();
            var cancellationWindowMinutes = _settingService.GetSetting("TRIP_CANCELLATION_WINDOW");
            var cancellationLimit = _settingService.GetSetting("TRIP_CANCELLATION_LIMIT");

            //var cancellationWindow = now.AddMinutes(-cancellationWindowMinutes);

            // Check if a driver was assigned to the trip
            if (trip.DriverId.HasValue)
            {
                var driver = await _unitOfWork.UserRepository.GetUserById(trip.DriverId.Value.ToString());
                if (driver != null && driver.Status == UserStatus.BUSY)
                {
                    // Reset the status of the driver to active, so he can continue receiving new request
                    driver.Status = UserStatus.ACTIVE;
                    driver.UpdatedTime = DateTimeUtilities.GetDateTimeVnNow();
                    await _unitOfWork.UserRepository.UpdateAsync(driver);
                }
            }

            // Remove driver from pending trip
            if (trip.Status == TripStatus.PENDING)
            {
                // Remove driver from the trip if he hasn't accepted the trip
                if (trip.Status == TripStatus.PENDING)
                {
                    trip.DriverId = null;
                }

                // Cancel find driver task
                _logger.LogInformation("Cancelling find driver task for tripId: {tripId}", trip.Id);
                KeyValueStore.Instance.Set($"CancelFindDriverTask_{trip.Id}", "true");
            }

            trip.Status = TripStatus.CANCELED;
            trip.UpdatedTime = DateTimeUtilities.GetDateTimeVnNow();

            await _unitOfWork.TripRepository.UpdateAsync(trip);

            booker.CanceledTripCount++;

            // Cancel reach limit
            var banDuration = _settingService.GetSetting("CANCELLATION_BAN_DURATION");

            if (booker.CanceledTripCount == 1)
            {
                booker.LastTripCancellationTime = now;

                // Scheduled job to reset cancellation count and time
                _logger.LogInformation("Scheduling job to reset trip cancellation for userId: {userId}", booker.Id);
                var resetJobId = BackgroundJob.Schedule<BackgroundServices>(s => s.ResetCancellationCountAndTime(booker.Id), TimeSpan.FromMinutes(cancellationWindowMinutes));
                KeyValueStore.Instance.Set($"ResetCancellationTask_{booker.Id}", resetJobId);
            }

            if (booker.CanceledTripCount == cancellationLimit)
            {
                booker.CancellationBanUntil = now.AddMinutes(banDuration);

                // Cancel the reset job as the user has reached the cancellation limit
                var resetJobId = KeyValueStore.Instance.Get<string>($"ResetCancellationTask_{booker.Id}");
                if (!string.IsNullOrEmpty(resetJobId))
                {
                    _logger.LogInformation("Cancelling reset job for userId: {userId} as cancellation limit has been reached", booker.Id);
                    BackgroundJob.Delete(resetJobId);
                    KeyValueStore.Instance.Remove($"ResetCancellationTask_{booker.Id}");
                }
            }

            await _unitOfWork.UserRepository.UpdateAsync(booker);

            // Change status of passenger back to inactive
            trip.Passenger.Status = UserStatus.INACTIVE;
            trip.Passenger.UpdatedTime = DateTimeUtilities.GetDateTimeVnNow();
            await _unitOfWork.UserRepository.UpdateAsync(trip.Passenger);

            await _unitOfWork.Save();

            // Notify passenger using FCM and SignalR
            await NotifyUsersTripCanceled(trip);

            return _mapper.Map<TripDto>(trip);
        }

        private async Task NotifyUsersTripCanceled(Trip trip)
        {
            // Guardian book for dependent
            if (trip.Type == TripType.BOOK_FOR_DEP_WITH_APP && trip.CanceledBy != null)
            {
                // Canceled by dependent
                if (trip.CanceledBy == trip.PassengerId)
                {
                    await NotifyUserWithFirebaseAsync(trip.Passenger.DeviceToken!,
                        "Chuyến đã bị hủy", 
                        "Bạn đã hủy chuyến thành công", 
                        trip.Passenger);
                    await NotifyUserWithFirebaseAsync(trip.Booker.DeviceToken!, 
                        "Chuyến đã bị hủy", 
                        $"Người thân {trip.Passenger.Name} đã hủy chuyến", 
                        trip.Booker);
                }
                // Canceled by booker/guardian
                else if (trip.CanceledBy == trip.BookerId)
                {
                    await NotifyUserWithFirebaseAsync(trip.Passenger.DeviceToken!, 
                        "Chuyến đã bị hủy", 
                        $"Người thân {trip.Passenger.Guardian!.Name} của bạn đã hủy chuyến", 
                        trip.Passenger);
                    await NotifyUserWithFirebaseAsync(trip.Booker.DeviceToken!, 
                        "Chuyến đã bị hủy", 
                        "Bạn đã hủy chuyến thành công", 
                        trip.Booker);
                }

                bool isSelfBooking = false;
                bool isNotificationForGuardian = true;
                bool isCanceledByAdmin = false;
                await _hubContext.Clients.Group(trip.Passenger.GuardianId.ToString())
                    .SendAsync("NotifyPassengerTripCanceled", _mapper.Map<TripDto>(trip), isSelfBooking, isNotificationForGuardian, isCanceledByAdmin);

                isNotificationForGuardian = false;
                await _hubContext.Clients.Group(trip.PassengerId.ToString())
                    .SendAsync("NotifyPassengerTripCanceled", _mapper.Map<TripDto>(trip), isSelfBooking, isNotificationForGuardian, isCanceledByAdmin);
            }
            else if (trip.CanceledBy == trip.PassengerId && trip.CanceledBy == trip.BookerId) // Self-book and book for dep no app
            {
                await NotifyUserWithFirebaseAsync(trip.Booker.DeviceToken!,
                        "Chuyến đã bị hủy",
                        "Bạn đã hủy chuyến thành công",
                        trip.Booker);

                bool isSelfBooking = true;
                bool isNotificationForGuardian = false;
                bool isCanceledByAdmin = false;
                await _hubContext.Clients.Group(trip.BookerId.ToString())
                    .SendAsync("NotifyPassengerTripCanceled", _mapper.Map<TripDto>(trip), isSelfBooking, isNotificationForGuardian, isCanceledByAdmin);
            }
            else if (trip.CanceledBy == null) // Admin cancel
            {
                bool isSelfBooking = false;
                bool isNotificationForGuardian = false;
                bool isCanceledByAdmin = true;

                await NotifyUserWithFirebaseAsync(trip.Driver!.DeviceToken!, 
                    "Chuyến đã bị hủy", 
                    "Chuyến đã bị quản trị viên hủy vì có vấn đề", 
                    trip.Driver);

                if (trip.Type == TripType.BOOK_FOR_DEP_WITH_APP)
                {
                    // Noti dep
                    await NotifyUserWithFirebaseAsync(trip.Passenger.DeviceToken!, 
                        "Chuyến đã bị hủy", 
                        "Chuyến đã bị quản trị viên hủy vì có vấn đề", 
                        trip.Passenger);

                    await _hubContext.Clients.Group(trip.PassengerId.ToString())
                        .SendAsync("NotifyPassengerTripCanceled", _mapper.Map<TripDto>(trip), isSelfBooking, isNotificationForGuardian, isCanceledByAdmin);
                }

                // Noti booker
                await NotifyUserWithFirebaseAsync(trip.Booker.DeviceToken!, 
                    "Chuyến đã bị hủy", 
                    "Chuyến đã bị quản trị viên hủy theo yêu cầu của bạn", 
                    trip.Booker);

                isNotificationForGuardian = true;
                await _hubContext.Clients.Group(trip.DriverId.ToString())
                        .SendAsync("NotifyPassengerTripCanceled", _mapper.Map<TripDto>(trip), isSelfBooking, isNotificationForGuardian, isCanceledByAdmin);
                await _hubContext.Clients.Group(trip.BookerId.ToString())
                        .SendAsync("NotifyPassengerTripCanceled", _mapper.Map<TripDto>(trip), isSelfBooking, isNotificationForGuardian, isCanceledByAdmin);
            }

            await _unitOfWork.Save();
        }

        private async Task NotifyUserWithFirebaseAsync(string deviceToken, string title, string content, User user)
        {
            if (!string.IsNullOrEmpty(deviceToken))
            {
                var result = await FirebaseUtilities.SendNotificationToDeviceAsync(deviceToken, title, content,
                    new Dictionary<string, string>
                    {
                        { "tripId", user.Id.ToString() }
                    });

                if (result == string.Empty)
                {
                    user.DeviceToken = null;
                    await _unitOfWork.UserRepository.UpdateAsync(user);
                }
            }
        }
    }
}