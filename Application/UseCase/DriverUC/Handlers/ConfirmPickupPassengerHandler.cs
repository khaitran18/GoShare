using Application.Common.Dtos;
using Application.Common.Exceptions;
using Application.Common.Utilities;
using Application.Common.Utilities.Google.Firebase;
using Application.Services.Interfaces;
using Application.SignalR;
using Application.UseCase.DriverUC.Commands;
using AutoMapper;
using Domain.DataModels;
using Domain.Enumerations;
using Domain.Interfaces;
using MediatR;
using Microsoft.AspNetCore.SignalR;

namespace Application.UseCase.DriverUC.Handlers
{
    public class ConfirmPickupPassengerHandler : IRequestHandler<ConfirmPickupPassengerCommand, TripDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ISettingService _settingService;
        private readonly UserClaims _userClaims;
        private readonly IHubContext<SignalRHub> _hubContext;
        private readonly IFirebaseStorage _firebaseStorage;

        public ConfirmPickupPassengerHandler(IUnitOfWork unitOfWork, IMapper mapper, ISettingService settingService, UserClaims userClaims, IHubContext<SignalRHub> hubContext, IFirebaseStorage firebaseStorage)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _settingService = settingService;
            _userClaims = userClaims;
            _hubContext = hubContext;
            _firebaseStorage = firebaseStorage;
        }

        public async Task<TripDto> Handle(ConfirmPickupPassengerCommand request, CancellationToken cancellationToken)
        {
            var tripDto = new TripDto();

            var trip = await _unitOfWork.TripRepository.GetByIdAsync(request.TripId);

            if (trip == null)
            {
                throw new NotFoundException(nameof(Trip), request.TripId);
            }

            if (trip.Status != TripStatus.GOING_TO_PICKUP)
            {
                throw new BadRequestException("The trip is invalid.");
            }

            Guid driverId = (Guid)_userClaims.id!;

            if (trip.DriverId != driverId)
            {
                throw new BadRequestException("The driver does not match for this trip.");
            }

            var driverLocation = await _unitOfWork.LocationRepository.GetByUserIdAndTypeAsync(driverId, LocationType.CURRENT_LOCATION);

            if (driverLocation == null)
            {
                throw new NotFoundException(nameof(Location), driverId);
            }

            driverLocation.Latitude = request.DriverLatitude;
            driverLocation.Longtitude = request.DriverLongitude;
            driverLocation.UpdatedTime = DateTimeUtilities.GetDateTimeVnNow();

            await _unitOfWork.LocationRepository.UpdateAsync(driverLocation);

            await _unitOfWork.Save();

            var startLocation = await _unitOfWork.LocationRepository.GetByIdAsync(trip.StartLocationId);

            if (startLocation == null)
            {
                throw new NotFoundException(nameof(Location), trip.StartLocationId);
            }

            var distance = MapsUtilities.GetDistance(driverLocation, startLocation);

            if (distance > _settingService.GetSetting("NEAR_DESTINATION_DISTANCE")) //km
            {
                throw new BadRequestException("The driver is not near the pickup location.");
            }

            // If the trip is book for dep no app, upload the image
            if (trip.Type == TripType.BOOK_FOR_DEP_NO_APP)
            {
                if (request.Image == null)
                {
                    throw new BadRequestException("Image is required as proof of pickup. Please upload an image.");
                }

                string path = trip.Id.ToString();
                string filename = trip.Id.ToString() + "_pickup";
                string url = await _firebaseStorage.UploadFileAsync(request.Image, path, filename);

                var tripImage = new TripImage
                {
                    Id = Guid.NewGuid(),
                    TripId = trip.Id,
                    ImageUrl = url,
                    Type = TripImageType.PICK_UP,
                    CreateTime = DateTimeUtilities.GetDateTimeVnNow(),
                    UpdatedTime = DateTimeUtilities.GetDateTimeVnNow()
                };

                await _unitOfWork.TripImageRepository.AddAsync(tripImage);
            }

            trip.Status = TripStatus.GOING;
            trip.PickupTime = DateTimeUtilities.GetDateTimeVnNow();
            trip.UpdatedTime = DateTimeUtilities.GetDateTimeVnNow();

            await _unitOfWork.TripRepository.UpdateAsync(trip);

            await _unitOfWork.Save();

            tripDto = _mapper.Map<TripDto>(trip);

            // Notify passenger using FCM and SignalR
            await NotifyPassengerAboutDriverOnTheWay(trip);

            return tripDto;
        }

        private async Task NotifyPassengerAboutDriverOnTheWay(Trip trip)
        {
            await NotifyUserWithFirebaseAsync(trip.Passenger!.DeviceToken!,
                    "Tài xế đã tới",
                    $"Tài xế {trip.Driver!.Name} đã đến địa điểm đón của bạn",
                    trip.Passenger);
            
            // Noti booker about image
            if (trip.Type == TripType.BOOK_FOR_DEP_NO_APP)
            {
                await NotifyUserWithFirebaseAsync(trip.Booker!.DeviceToken!,
                    "Chuyến có ảnh mới",
                    $"Tài xế vừa gửi ảnh người thân {trip.PassengerName} của bạn tại điểm đón.",
                    trip.Booker);
            }

            if (trip.Type == TripType.BOOK_FOR_DEP_WITH_APP)
            {
                await NotifyUserWithFirebaseAsync(trip.Booker!.DeviceToken!,
                    "Tài xế đã tới",
                    $"Tài xế {trip.Driver!.Name} đã đến địa điểm đón người thân của bạn",
                    trip.Booker);

                bool isSelfBooking = false;
                bool isNotificationForGuardian = true;
                await _hubContext.Clients.Group(trip.Passenger.GuardianId.ToString())
                    .SendAsync("NotifyPassengerDriverPickup", _mapper.Map<TripDto>(trip), isSelfBooking, isNotificationForGuardian);

                isNotificationForGuardian = false;
                await _hubContext.Clients.Group(trip.PassengerId.ToString())
                    .SendAsync("NotifyPassengerDriverPickup", _mapper.Map<TripDto>(trip), isSelfBooking, isNotificationForGuardian);
            }
            else // selfbook and book for dep no app
            {
                bool isSelfBooking = true;
                bool isNotificationForGuardian = false;
                await _hubContext.Clients.Group(trip.PassengerId.ToString())
                    .SendAsync("NotifyPassengerDriverPickup", _mapper.Map<TripDto>(trip), isSelfBooking, isNotificationForGuardian);
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
