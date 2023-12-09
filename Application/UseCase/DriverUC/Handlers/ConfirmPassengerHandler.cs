using Application.Common.Dtos;
using Application.Common.Exceptions;
using Application.Common.Utilities;
using Application.Services.Interfaces;
using Application.UseCase.DriverUC.Commands;
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

namespace Application.UseCase.DriverUC.Handlers
{
    public class ConfirmPassengerHandler : IRequestHandler<ConfirmPassengerCommand, TripDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ISettingService _settingService;
        private readonly IMapper _mapper;
        private readonly UserClaims _userClaims;

        public ConfirmPassengerHandler(IUnitOfWork unitOfWork, ISettingService settingService, IMapper mapper, UserClaims userClaims)
        {
            _unitOfWork = unitOfWork;
            _settingService = settingService;
            _mapper = mapper;
            _userClaims = userClaims;
        }

        public async Task<TripDto> Handle(ConfirmPassengerCommand request, CancellationToken cancellationToken)
        {
            var trip = await _unitOfWork.TripRepository.GetByIdAsync(request.TripId);
            Guid driverId = (Guid)_userClaims.id!;

            if (trip == null)
            {
                throw new NotFoundException(nameof(Trip), request.TripId);
            }

            if (trip.Status != TripStatus.PENDING)
            {
                throw new BadRequestException("The trip is invalid.");
            }

            if (trip.DriverId != driverId)
            {
                throw new BadRequestException("Driver is not assigned to this trip.");
            }

            if (request.Accept)
            {
                var driver = await _unitOfWork.UserRepository.GetUserById(driverId.ToString());

                if (driver == null)
                {
                    throw new NotFoundException(nameof(User), driverId);
                }

                var car = await _unitOfWork.CarRepository.GetByUserId(driverId);

                if (car == null)
                {
                    throw new NotFoundException(nameof(Car), driverId);
                }

                if (car.TypeId != trip.CartypeId)
                {
                    throw new BadRequestException("The driver's car type does not match the trip's car type.");
                }

                trip.Status = TripStatus.GOING_TO_PICKUP;
                trip.UpdatedTime = DateTimeUtilities.GetDateTimeVnNow();

                await _unitOfWork.TripRepository.UpdateAsync(trip);

                // Wallet transaction
                if (trip.PaymentMethod == PaymentMethod.WALLET)
                {
                    Guid walletOwnerId = trip.BookerId;
                    var walletOwnerWallet = await _unitOfWork.WalletRepository.GetByUserIdAsync(walletOwnerId);
                    // These validation shouldn't happen, but I place them here just in case
                    if (walletOwnerWallet == null)
                    {
                        throw new NotFoundException(nameof(Wallet), walletOwnerId);
                    }
                    if (walletOwnerWallet.Balance < trip.Price)
                    {
                        throw new BadRequestException("The wallet owner's wallet does not have enough balance.");
                    }

                    walletOwnerWallet.Balance -= trip.Price;
                    walletOwnerWallet.UpdatedTime = DateTimeUtilities.GetDateTimeVnNow();
                    await _unitOfWork.WalletRepository.UpdateAsync(walletOwnerWallet);

                    // New transaction for user's wallet
                    var userTransaction = new Wallettransaction
                    {
                        Id = Guid.NewGuid(),
                        WalletId = walletOwnerWallet.Id,
                        TripId = trip.Id,
                        Amount = -trip.Price, // Negative amount for wallet payment
                        PaymentMethod = PaymentMethod.WALLET,
                        Status = WalletTransactionStatus.SUCCESSFULL,
                        Type = WalletTransactionType.PASSENGER_PAYMENT,
                        CreateTime = DateTimeUtilities.GetDateTimeVnNow(),
                        UpdatedTime = DateTimeUtilities.GetDateTimeVnNow()
                    };

                    await _unitOfWork.WallettransactionRepository.AddAsync(userTransaction);
                }

                await _unitOfWork.Save();

                KeyValueStore.Instance.Set($"TripConfirmationTask_{trip.Id}", "true");
            }
            else
            {
                KeyValueStore.Instance.Set($"TripConfirmationTask_{trip.Id}", "false");
            }

            return _mapper.Map<TripDto>(trip);
        }
    }
}
