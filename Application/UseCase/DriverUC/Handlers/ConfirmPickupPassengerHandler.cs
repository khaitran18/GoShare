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

        public ConfirmPickupPassengerHandler(IUnitOfWork unitOfWork, IMapper mapper, ISettingService settingService, UserClaims userClaims, IHubContext<SignalRHub> hubContext)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _settingService = settingService;
            _userClaims = userClaims;
            _hubContext = hubContext;
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

        private async Task NotifyPassengerAboutDriverOnTheWay(Domain.DataModels.Trip trip)
        {

            if (!string.IsNullOrEmpty(trip.Passenger.DeviceToken))
            {
                var result = await FirebaseUtilities.SendNotificationToDeviceAsync(trip.Passenger.DeviceToken,
                    "Tài xế đã tới",
                    $"Tài xế {trip.Driver!.Name} đã đến địa điểm đón của bạn",
                    new Dictionary<string, string>
                    {
                        { "tripId", trip.Id.ToString() }
                    });

                if (result == string.Empty)
                {
                    trip.Passenger.DeviceToken = null;
                    await _unitOfWork.UserRepository.UpdateAsync(trip.Passenger);
                }
            }


            if (trip.Passenger.GuardianId != null && trip.Passenger.GuardianId == trip.BookerId)
            {
                if (!string.IsNullOrEmpty(trip.Passenger.Guardian!.DeviceToken))
                {
                    var result = await FirebaseUtilities.SendNotificationToDeviceAsync(trip.Passenger.Guardian.DeviceToken,
                        "Tài xế đã tới",
                        $"Tài xế {trip.Driver!.Name} đã đến địa điểm đón người thân của bạn",
                        new Dictionary<string, string>
                        {
                            { "tripId", trip.Id.ToString() }
                        });

                    if (result == string.Empty)
                    {
                        trip.Passenger.Guardian.DeviceToken = null;
                        await _unitOfWork.UserRepository.UpdateAsync(trip.Passenger.Guardian);
                    }
                }

                bool isSelfBooking = false;
                bool isNotificationForGuardian = true;
                await _hubContext.Clients.Group(trip.Passenger.GuardianId.ToString())
                    .SendAsync("NotifyPassengerDriverPickup", _mapper.Map<TripDto>(trip), isSelfBooking, isNotificationForGuardian);

                isNotificationForGuardian = false;
                await _hubContext.Clients.Group(trip.PassengerId.ToString())
                    .SendAsync("NotifyPassengerDriverPickup", _mapper.Map<TripDto>(trip), isSelfBooking, isNotificationForGuardian);
            }
            else
            {
                bool isSelfBooking = true;
                bool isNotificationForGuardian = false;
                await _hubContext.Clients.Group(trip.PassengerId.ToString())
                    .SendAsync("NotifyPassengerDriverPickup", _mapper.Map<TripDto>(trip), isSelfBooking, isNotificationForGuardian);
            }

            await _unitOfWork.Save();
        }
    }
}
