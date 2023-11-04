using Application.Common.Dtos;
using Application.Common.Exceptions;
using Application.Common.Utilities;
using Application.Services.Interfaces;
using AutoMapper;
using Domain.DataModels;
using Domain.Enumerations;
using Domain.Interfaces;
using MediatR;
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
        private readonly ITokenService _tokenService;
        private readonly IMapper _mapper;
        private readonly ISettingService _settingService;

        public EndTripHandler(IUnitOfWork unitOfWork, ITokenService tokenService, IMapper mapper, ISettingService settingService)
        {
            _unitOfWork = unitOfWork;
            _tokenService = tokenService;
            _mapper = mapper;
            _settingService = settingService;
        }

        public async Task<TripDto> Handle(EndTripCommand request, CancellationToken cancellationToken)
        {
            var tripDto = new TripDto();

            ClaimsPrincipal? claims = _tokenService.ValidateToken(request.Token ?? "");
            Guid.TryParse(claims!.FindFirst("id")?.Value, out Guid driverId);

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
            driverLocation.UpdatedTime = DateTime.Now;

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
            trip.EndTime = DateTime.Now;
            trip.UpdatedTime = DateTime.Now;

            await _unitOfWork.TripRepository.UpdateAsync(trip);

            // Set status of driver back to active
            var driver = await _unitOfWork.UserRepository.GetUserById(driverId.ToString());
            if (driver != null)
            {
                driver.Status = UserStatus.ACTIVE;
                driver.UpdatedTime = DateTime.Now;
                await _unitOfWork.UserRepository.UpdateAsync(driver);
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
                driverWallet.UpdatedTime = DateTime.Now;
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
                    CreateTime = DateTime.Now,
                    UpdatedTime = DateTime.Now
                };

                await _unitOfWork.WallettransactionRepository.AddAsync(driverTransaction);

                driverWallet.Balance += driverWage;
                driverWallet.UpdatedTime = DateTime.Now;
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
                    CreateTime = DateTime.Now,
                    UpdatedTime = DateTime.Now
                };

                await _unitOfWork.WallettransactionRepository.AddAsync(systemTransaction);

                systemWallet.Balance += systemCommission;
                systemWallet.UpdatedTime = DateTime.Now;
                await _unitOfWork.WalletRepository.UpdateAsync(systemWallet);
            }

            await _unitOfWork.Save();

            tripDto = _mapper.Map<TripDto>(trip);

            return tripDto;
        }
    }
}
