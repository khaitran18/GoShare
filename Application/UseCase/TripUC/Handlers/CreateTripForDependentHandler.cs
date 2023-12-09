using Application.Common.Dtos;
using Application.Common.Exceptions;
using Application.Common.Utilities;
using Application.Common.Utilities.Google;
using Application.Common.Utilities.Google.Firebase;
using Application.Common.Utilities.SignalR;
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
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCase.TripUC.Handlers
{
    public class CreateTripForDependentHandler : IRequestHandler<CreateTripForDependentCommand, TripDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly UserClaims _userClaims;
        private readonly IServiceProvider _serviceProvider;
        private readonly ISettingService _settingService;
        private readonly IHubContext<SignalRHub> _hubContext;

        public CreateTripForDependentHandler(IUnitOfWork unitOfWork, IMapper mapper, UserClaims userClaims, IServiceProvider serviceProvider, ISettingService settingService, IHubContext<SignalRHub> hubContext)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _userClaims = userClaims;
            _serviceProvider = serviceProvider;
            _settingService = settingService;
            _hubContext = hubContext;
        }

        public async Task<TripDto> Handle(CreateTripForDependentCommand request, CancellationToken cancellationToken)
        {
            var tripDto = new TripDto();

            Guid userId = (Guid)_userClaims.id!;

            var guardian = await _unitOfWork.UserRepository.GetUserById(userId.ToString());
            if (guardian == null)
            {
                throw new NotFoundException(nameof(User), userId);
            }
            // Check if user is a guardian
            if (guardian.GuardianId != null)
            {
                throw new BadRequestException("You are not allow to do this function");
            }

            var dependent = await _unitOfWork.UserRepository.GetUserById(request.DependentId.ToString());
            if (dependent == null)
            {
                throw new NotFoundException(nameof(User), request.DependentId);
            }
            // Check if dependent is of the user
            if (dependent.GuardianId != userId)
            {
                throw new BadRequestException("The user is not the guardian of the dependent.");
            }

            // Check if the passenger is already in a trip that hasn't completed
            var ongoingTrip = await _unitOfWork.TripRepository.GetOngoingTripByPassengerId(request.DependentId);
            if (ongoingTrip != null)
            {
                throw new BadRequestException("Passenger is already in a trip that hasn't completed. Please complete the current trip before creating a new one.");
            }

            // Prevent creating trip for dependents who are busy
            if (dependent.Status == UserStatus.BUSY)
            {
                throw new BadRequestException("This dependent is already in a trip.");
            }

            var now = DateTimeUtilities.GetDateTimeVnNow();
            var cancellationWindowMinutes = _settingService.GetSetting("TRIP_CANCELLATION_WINDOW");
            //var cancellationLimit = _settingService.GetSetting("TRIP_CANCELLATION_LIMIT");

            //var cancellationWindow = now.AddMinutes(-cancellationWindowMinutes);

            if (guardian.CancellationBanUntil != null)
            {
                // Check if the guardian has cancelled too many trips recently (being blocked)
                if (now < guardian.CancellationBanUntil)
                {
                    throw new BadRequestException($"You have exceeded the maximum number of cancellations allowed within {cancellationWindowMinutes} minutes. " +
                        $"Please wait until {guardian.CancellationBanUntil} before creating a new trip.");
                }
                else
                {
                    // If the ban duration has passed, reset
                    guardian.CanceledTripCount = 0;
                    guardian.LastTripCancellationTime = null;
                    guardian.CancellationBanUntil = null;
                    guardian.UpdatedTime = DateTimeUtilities.GetDateTimeVnNow();
                    await _unitOfWork.UserRepository.UpdateAsync(guardian);
                }
            }

            var currentLocation = await _unitOfWork.LocationRepository.GetByUserIdAndTypeAsync(request.DependentId, LocationType.CURRENT_LOCATION);
            if (currentLocation == null)
            {
                throw new NotFoundException(nameof(Location), request.DependentId);
            }

            // Check if a past origin with the same coordinates already exists
            var dependentPastOrigin = await _unitOfWork.LocationRepository
                .GetByUserIdAndLatLongAndTypeAsync(request.DependentId, currentLocation.Latitude, currentLocation.Longtitude, LocationType.PAST_ORIGIN);

            if (dependentPastOrigin == null)
            {
                // If not, create a new past origin location
                dependentPastOrigin = new Location
                {
                    Id = Guid.NewGuid(),
                    UserId = request.DependentId,
                    Address = currentLocation.Address,
                    Latitude = currentLocation.Latitude,
                    Longtitude = currentLocation.Longtitude,
                    Type = LocationType.PAST_ORIGIN,
                    CreateTime = DateTimeUtilities.GetDateTimeVnNow(),
                    UpdatedTime = DateTimeUtilities.GetDateTimeVnNow()
                };

                await _unitOfWork.LocationRepository.AddAsync(dependentPastOrigin);
            }
            else
            {
                // If a past origin with the same coordinates exists, update its information
                dependentPastOrigin.Address = currentLocation.Address;
                dependentPastOrigin.UpdatedTime = DateTimeUtilities.GetDateTimeVnNow();

                await _unitOfWork.LocationRepository.UpdateAsync(dependentPastOrigin);
            }

            // Same for destination
            var destination = await _unitOfWork.LocationRepository
                .GetByUserIdAndLatLongAndTypeAsync(request.DependentId, request.EndLatitude, request.EndLongitude, LocationType.PAST_DESTINATION);

            if (destination == null)
            {
                // If not, create a new destination location
                destination = new Location
                {
                    Id = Guid.NewGuid(),
                    UserId = request.DependentId,
                    Address = request.EndAddress,
                    Latitude = request.EndLatitude,
                    Longtitude = request.EndLongitude,
                    Type = LocationType.PAST_DESTINATION,
                    CreateTime = DateTimeUtilities.GetDateTimeVnNow(),
                    UpdatedTime = DateTimeUtilities.GetDateTimeVnNow()
                };

                await _unitOfWork.LocationRepository.AddAsync(destination);
            }
            else
            {
                // If a destination with the same coordinates exists, update its information
                destination.Address = request.EndAddress;
                destination.UpdatedTime = DateTimeUtilities.GetDateTimeVnNow();

                await _unitOfWork.LocationRepository.UpdateAsync(destination);
            }

            var distance = await GoogleMapsApiUtilities.ComputeDistanceMatrixAsync(dependentPastOrigin, destination);

            var totalPrice = await _unitOfWork.CartypeRepository.CalculatePriceForCarType(request.CartypeId, distance);

            if (request.PaymentMethod == PaymentMethod.WALLET)
            {
                var walletOwnerWallet = await _unitOfWork.WalletRepository.GetByUserIdAsync(userId);
                if (walletOwnerWallet == null)
                {
                    throw new NotFoundException(nameof(Wallet), userId);
                }

                if (walletOwnerWallet.Balance < totalPrice)
                {
                    throw new BadRequestException("The wallet owner's wallet does not have enough balance.");
                }
            }

            var trip = new Trip
            {
                Id = Guid.NewGuid(),
                PassengerId = request.DependentId,
                StartLocationId = dependentPastOrigin.Id,
                EndLocationId = destination.Id,
                StartTime = DateTimeUtilities.GetDateTimeVnNow(),
                CreateTime = DateTimeUtilities.GetDateTimeVnNow(),
                UpdatedTime = DateTimeUtilities.GetDateTimeVnNow(),
                Distance = distance,
                CartypeId = request.CartypeId,
                Price = totalPrice,
                Status = TripStatus.PENDING,
                PaymentMethod = request.PaymentMethod,
                BookerId = userId,
                Note = request.Note,
                PassengerName = dependent.Name,
                PassengerPhoneNumber = dependent.Phone,
                Type = TripType.BOOK_FOR_DEP_WITH_APP
            };

            await _unitOfWork.TripRepository.AddAsync(trip);

            // Set status of dependent to busy
            dependent.Status = UserStatus.BUSY;
            dependent.UpdatedTime = DateTimeUtilities.GetDateTimeVnNow();
            await _unitOfWork.UserRepository.UpdateAsync(dependent);

            await _unitOfWork.Save();

            tripDto = _mapper.Map<TripDto>(trip);

            // Background task find driver
            string jobId = BackgroundJob.Enqueue<BackgroundServices>(s => s.FindDriver(trip.Id, request.CartypeId));

            // Notify dependent using FCM and SignalR
            await NotifyDependentNewTripBooked(trip);

            return tripDto;
        }

        private async Task NotifyDependentNewTripBooked(Trip trip)
        {
            if (!string.IsNullOrEmpty(trip.Passenger.DeviceToken))
            {
                var result = await FirebaseUtilities.SendNotificationToDeviceAsync(trip.Passenger.DeviceToken,
                    "Đã tạo chuyến mới",
                    $"Người thân {trip.Booker!.Name} đã tạo chuyến mới cho bạn",
                    new Dictionary<string, string>
                    {
                        { "tripId", trip.Id.ToString() }
                    });

                if (result == string.Empty)
                {
                    trip.Passenger.DeviceToken = null;
                    await _unitOfWork.UserRepository.UpdateAsync(trip.Passenger);
                    await _unitOfWork.Save();
                }
            }

            await _hubContext.Clients.Group(trip.PassengerId.ToString())
                    .SendAsync("NotifyDependentNewTripBooked", _mapper.Map<TripDto>(trip));
        }
    }
}
