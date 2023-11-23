using Application.Common.Dtos;
using Application.Common.Exceptions;
using Application.Common.Utilities;
using Application.Common.Utilities.Google.Firebase;
using Application.Common.Utilities.SignalR;
using Application.Services.Interfaces;
using Application.SignalR;
using AutoMapper;
using Domain.DataModels;
using Domain.Enumerations;
using Domain.Interfaces;
using MediatR;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Application.Commands.Handlers
{
    public class EndTripHandler : IRequestHandler<EndTripCommand, TripDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserClaims _userClaims;
        private readonly IMapper _mapper;
        private readonly ISettingService _settingService;
        private readonly IHubContext<SignalRHub> _hubContext;

        public EndTripHandler(IUnitOfWork unitOfWork, UserClaims userClaims, IMapper mapper, ISettingService settingService, IHubContext<SignalRHub> hubContext)
        {
            _unitOfWork = unitOfWork;
            _userClaims = userClaims;
            _mapper = mapper;
            _settingService = settingService;
            _hubContext = hubContext;
        }

        public async Task<TripDto> Handle(EndTripCommand request, CancellationToken cancellationToken)
        {
            var tripDto = new TripDto();

            Guid driverId = (Guid)_userClaims.id!;

            var trip = await _unitOfWork.TripRepository.GetByIdAsync(request.TripId);

            if (trip == null)
            {
                throw new NotFoundException(nameof(Trip), request.TripId);
            }

            if (trip.Status != TripStatus.GOING)
            {
                throw new BadRequestException("The trip is invalid.");
            }

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

            var endLocation = await _unitOfWork.LocationRepository.GetByIdAsync(trip.EndLocationId);

            if (endLocation == null)
            {
                throw new NotFoundException(nameof(Location), trip.EndLocationId);
            }

            var distance = MapsUtilities.GetDistance(driverLocation, endLocation);

            if (distance > _settingService.GetSetting("NEAR_DESTINATION_DISTANCE")) //km
            {
                throw new BadRequestException("The driver is not near the drop-off location.");
            }

            trip.Status = TripStatus.COMPLETED;
            trip.EndTime = DateTimeUtilities.GetDateTimeVnNow();
            trip.UpdatedTime = DateTimeUtilities.GetDateTimeVnNow();

            await _unitOfWork.TripRepository.UpdateAsync(trip);

            // Set status of driver back to active
            var driver = await _unitOfWork.UserRepository.GetUserById(driverId.ToString());
            if (driver != null)
            {
                driver.Status = UserStatus.ACTIVE;
                driver.UpdatedTime = DateTimeUtilities.GetDateTimeVnNow();
                await _unitOfWork.UserRepository.UpdateAsync(driver);
            }

            // Set status of passenger back to inactive
            var passenger = await _unitOfWork.UserRepository.GetUserById(trip.PassengerId.ToString());
            if (passenger != null)
            {
                passenger.Status = UserStatus.INACTIVE;
                passenger.UpdatedTime = DateTimeUtilities.GetDateTimeVnNow();
                await _unitOfWork.UserRepository.UpdateAsync(passenger);
            }

            // Payment
            var driverWallet = await _unitOfWork.WalletRepository.GetByUserIdAsync(driverId);
            if (driverWallet == null)
            {
                throw new NotFoundException(nameof(Wallet), driverId);
            }

            var systemWallet = await _unitOfWork.WalletRepository.GetSystemWalletAsync();
            if (systemWallet == null)
            {
                throw new NotFoundException(nameof(Wallet), "System");
            }

            if (trip.PaymentMethod == PaymentMethod.CASH)
            {
                driverWallet.Balance -= trip.Price;
                driverWallet.UpdatedTime = DateTimeUtilities.GetDateTimeVnNow();
                await _unitOfWork.WalletRepository.UpdateAsync(driverWallet);
            }
            else if (trip.PaymentMethod == PaymentMethod.WALLET)
            {
                // Wallet transaction

                double driverWage = trip.Price * (_settingService.GetSetting("DRIVER_WAGE_PERCENT") / 100.0);

                var driverTransaction = new Wallettransaction
                {
                    Id = Guid.NewGuid(),
                    WalletId = driverWallet.Id,
                    TripId = trip.Id,
                    Amount = driverWage,
                    PaymentMethod = PaymentMethod.WALLET,
                    Status = WalletTransactionStatus.SUCCESSFULL,
                    Type = WalletTransactionType.DRIVER_WAGE,
                    CreateTime = DateTimeUtilities.GetDateTimeVnNow(),
                    UpdatedTime = DateTimeUtilities.GetDateTimeVnNow()
                };

                await _unitOfWork.WallettransactionRepository.AddAsync(driverTransaction);

                driverWallet.Balance += driverWage;
                driverWallet.UpdatedTime = DateTimeUtilities.GetDateTimeVnNow();
                await _unitOfWork.WalletRepository.UpdateAsync(driverWallet);

                double systemCommission = trip.Price - driverWage;

                var systemTransaction = new Wallettransaction
                {
                    Id = Guid.NewGuid(),
                    WalletId = systemWallet.Id,
                    TripId = trip.Id,
                    Amount = systemCommission,
                    PaymentMethod = PaymentMethod.WALLET,
                    Status = WalletTransactionStatus.SUCCESSFULL,
                    Type = WalletTransactionType.SYSTEM_COMMISSION,
                    CreateTime = DateTimeUtilities.GetDateTimeVnNow(),
                    UpdatedTime = DateTimeUtilities.GetDateTimeVnNow()
                };

                await _unitOfWork.WallettransactionRepository.AddAsync(systemTransaction);

                systemWallet.Balance += systemCommission;
                systemWallet.UpdatedTime = DateTimeUtilities.GetDateTimeVnNow();
                await _unitOfWork.WalletRepository.UpdateAsync(systemWallet);
            }

            await _unitOfWork.Save();

            tripDto = _mapper.Map<TripDto>(trip);

            // Notify passenger using FCM and SignalR
            await NotifyPassengerTripHasEnded(trip);

            return tripDto;
        }

        private async Task NotifyPassengerTripHasEnded(Trip trip)
        {
            if (!string.IsNullOrEmpty(trip.Passenger.DeviceToken))
            {
                await FirebaseUtilities.SendNotificationToDeviceAsync(trip.Passenger.DeviceToken,
                    "Hoàn thành chuyến",
                    $"Chuyến đi của bạn đã kết thúc. Bạn đã đến nơi an toàn.",
                    new Dictionary<string, string>
                    {
                        { "tripId", trip.Id.ToString() }
                    });
            }

            if (trip.Passenger.GuardianId != null)
            {
                if (!string.IsNullOrEmpty(trip.Passenger.Guardian!.DeviceToken))
                {
                    string message;
                    if (trip.StartLocation.Address == null && trip.EndLocation.Address != null)
                    {
                        message = $"Người thân {trip.Passenger.Name} vừa hoàn thành một chuyến đi đến {trip.EndLocation.Address}.";
                    }
                    else if (trip.StartLocation.Address != null && trip.EndLocation.Address == null)
                    {
                        message = $"Người thân {trip.Passenger.Name} vừa hoàn thành một chuyến đi từ {trip.StartLocation.Address}.";
                    }
                    else if (trip.StartLocation.Address == null && trip.EndLocation.Address == null)
                    {
                        message = $"Người thân {trip.Passenger.Name} vừa hoàn thành một chuyến đi.";
                    }
                    else
                    {
                        message = $"Người thân {trip.Passenger.Name} vừa hoàn thành chuyến đi từ {trip.StartLocation.Address} đến {trip.EndLocation.Address}.";
                    }

                    await FirebaseUtilities.SendNotificationToDeviceAsync(trip.Passenger.Guardian.DeviceToken,
                        "Một chuyến đi đã hoàn thành",
                        message,
                        new Dictionary<string, string>
                        {
                            { "tripId", trip.Id.ToString() }
                        });
                }
            }

            await _hubContext.Clients.Group(SignalRUtilities.GetGroupNameForUser(trip.Passenger, trip))
                    .SendAsync("NotifyPassengerTripEnded", _mapper.Map<TripDto>(trip));
        }
    }
}
