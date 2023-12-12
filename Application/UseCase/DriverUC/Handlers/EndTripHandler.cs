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
        private readonly IFirebaseStorage _firebaseStorage;

        public EndTripHandler(IUnitOfWork unitOfWork, UserClaims userClaims, IMapper mapper, ISettingService settingService, IHubContext<SignalRHub> hubContext, IFirebaseStorage firebaseStorage)
        {
            _unitOfWork = unitOfWork;
            _userClaims = userClaims;
            _mapper = mapper;
            _settingService = settingService;
            _hubContext = hubContext;
            _firebaseStorage = firebaseStorage;
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

            // If the trip is book for dep no app, upload the image
            if (trip.Type == TripType.BOOK_FOR_DEP_NO_APP)
            {
                if (request.Image == null)
                {
                    throw new BadRequestException("Image is required as proof of end trip. Please upload an image.");
                }

                string path = trip.Id.ToString();
                string filename = trip.Id.ToString() + "_end";
                string url = await _firebaseStorage.UploadFileAsync(request.Image, path, filename);

                var tripImage = new TripImage
                {
                    Id = Guid.NewGuid(),
                    TripId = trip.Id,
                    ImageUrl = url,
                    Type = TripImageType.END_TRIP,
                    CreateTime = DateTimeUtilities.GetDateTimeVnNow(),
                    UpdatedTime = DateTimeUtilities.GetDateTimeVnNow()
                };

                await _unitOfWork.TripImageRepository.AddAsync(tripImage);
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

            // Check driver wallet after it has been updated
            if (driverWallet.DueDate == null && driverWallet.Balance < _settingService.GetSetting("MINIMUM_BALANCE_LIMIT"))
            {
                // Set the deadline for the driver to top up the wallet
                var debtPaymentPeriod = _settingService.GetSetting("DEBT_REPAYMENT_PERIOD");
                driverWallet.DueDate = DateTimeUtilities.GetDateTimeVnNow().AddDays(debtPaymentPeriod);
                await _unitOfWork.WalletRepository.UpdateAsync(driverWallet);
            }

            // Check if the driver’s wallet is above 0
            else if (driverWallet.DueDate != null && driverWallet.Balance >= _settingService.GetSetting("BALANCE_THRESHOLD"))
            {
                // Reset the debt deadline
                driverWallet.DueDate = null;
                await _unitOfWork.WalletRepository.UpdateAsync(driverWallet);
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
            await NotifyUserWithFirebaseAsync(trip.Passenger!.DeviceToken!,
                    "Hoàn thành chuyến",
                    "Chuyến đi của bạn đã kết thúc. Bạn đã đến nơi an toàn.",
                    trip.Passenger);

            // Noti booker about image
            if (trip.Type == TripType.BOOK_FOR_DEP_NO_APP)
            {
                await NotifyUserWithFirebaseAsync(trip.Booker!.DeviceToken!,
                    "Chuyến có ảnh mới",
                    $"Tài xế vừa gửi ảnh người thân {trip.PassengerName} của bạn tại điểm đến.",
                    trip.Booker);
            }

            if (trip.Type == TripType.BOOK_FOR_DEP_WITH_APP)
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

                await NotifyUserWithFirebaseAsync(trip.Booker!.DeviceToken!,
                    "Một chuyến đi đã hoàn thành",
                    message,
                    trip.Booker);

                bool isSelfBooking = false;
                bool isNotificationForGuardian;

                isNotificationForGuardian = true;
                await _hubContext.Clients.Group(trip.Passenger.GuardianId.ToString())
                    .SendAsync("NotifyPassengerTripEnded", _mapper.Map<TripDto>(trip), isSelfBooking, isNotificationForGuardian);

                isNotificationForGuardian = false;
                await _hubContext.Clients.Group(trip.PassengerId.ToString())
                    .SendAsync("NotifyPassengerTripEnded", _mapper.Map<TripDto>(trip), isSelfBooking, isNotificationForGuardian);
            }
            else // Selfbook and book for dep no app
            {
                bool isSelfBooking = true;
                bool isNotificationForGuardian = false;
                await _hubContext.Clients.Group(trip.PassengerId.ToString())
                    .SendAsync("NotifyPassengerTripEnded", _mapper.Map<TripDto>(trip), isSelfBooking, isNotificationForGuardian);
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
