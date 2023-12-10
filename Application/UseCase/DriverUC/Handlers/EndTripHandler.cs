﻿using Application.Common.Dtos;
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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCase.DriverUC.Handlers
{
    public class EndTripHandler : IRequestHandler<EndTripCommand, TripEndDto>
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

        public async Task<TripEndDto> Handle(EndTripCommand request, CancellationToken cancellationToken)
        {
            var tripDto = new TripEndDto();

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

            double driverWage = trip.Price * (_settingService.GetSetting("DRIVER_WAGE_PERCENT") / 100.0);
            double systemCommission = trip.Price - driverWage;

            if (trip.PaymentMethod == PaymentMethod.CASH)
            {
                // New transaction for driver's wallet
                var driverCashTransaction = new Wallettransaction
                {
                    Id = Guid.NewGuid(),
                    WalletId = driverWallet.Id,
                    TripId = trip.Id,
                    Amount = -trip.Price, // Negative amount for cash payment
                    PaymentMethod = PaymentMethod.CASH,
                    Status = WalletTransactionStatus.SUCCESSFULL,
                    Type = WalletTransactionType.DRIVER_WAGE,
                    CreateTime = DateTimeUtilities.GetDateTimeVnNow(),
                    UpdatedTime = DateTimeUtilities.GetDateTimeVnNow()
                };

                await _unitOfWork.WallettransactionRepository.AddAsync(driverCashTransaction);

                driverWallet.Balance -= trip.Price;
                driverWallet.UpdatedTime = DateTimeUtilities.GetDateTimeVnNow();
                await _unitOfWork.WalletRepository.UpdateAsync(driverWallet);
            }
            else if (trip.PaymentMethod == PaymentMethod.WALLET)
            {
                // Wallet transaction

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

            tripDto = _mapper.Map<TripEndDto>(trip);
            tripDto.SystemCommission = systemCommission;

            // Notify passenger using FCM and SignalR
            await NotifyPassengerTripHasEnded(trip);

            return tripDto;
        }

        private async Task NotifyPassengerTripHasEnded(Trip trip)
        {
            if (!string.IsNullOrEmpty(trip.Passenger.DeviceToken))
            {
                var result = await FirebaseUtilities.SendNotificationToDeviceAsync(trip.Passenger.DeviceToken,
                    "Hoàn thành chuyến",
                    $"Chuyến đi của bạn đã kết thúc. Bạn đã đến nơi an toàn.",
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

                    var result = await FirebaseUtilities.SendNotificationToDeviceAsync(trip.Passenger.Guardian.DeviceToken,
                        "Một chuyến đi đã hoàn thành",
                        message,
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
                bool isNotificationForGuardian;

                if (trip.Passenger.GuardianId == trip.BookerId)
                {
                    isNotificationForGuardian = true;
                    await _hubContext.Clients.Group(trip.Passenger.GuardianId.ToString())
                        .SendAsync("NotifyPassengerTripEnded", _mapper.Map<TripDto>(trip), isSelfBooking, isNotificationForGuardian);

                    isNotificationForGuardian = false;
                    await _hubContext.Clients.Group(trip.PassengerId.ToString())
                        .SendAsync("NotifyPassengerTripEnded", _mapper.Map<TripDto>(trip), isSelfBooking, isNotificationForGuardian);
                }
                else
                {
                    isSelfBooking = true;
                    isNotificationForGuardian = false;
                    await _hubContext.Clients.Group(trip.PassengerId.ToString())
                        .SendAsync("NotifyPassengerTripEnded", _mapper.Map<TripDto>(trip), isSelfBooking, isNotificationForGuardian);
                }
            }
            else
            {
                bool isSelfBooking = true;
                bool isNotificationForGuardian = false;
                await _hubContext.Clients.Group(trip.PassengerId.ToString())
                    .SendAsync("NotifyPassengerTripEnded", _mapper.Map<TripDto>(trip), isSelfBooking, isNotificationForGuardian);
            }

            await _unitOfWork.Save();
        }
    }
}