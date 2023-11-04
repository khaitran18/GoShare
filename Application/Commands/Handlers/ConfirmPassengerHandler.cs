using Application.Common.Exceptions;
using Application.Common.Utilities;
using Application.Services.Interfaces;
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
    public class ConfirmPassengerHandler : IRequestHandler<ConfirmPassengerCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITokenService _tokenService;
        private readonly ISettingService _settingService;

        public ConfirmPassengerHandler(IUnitOfWork unitOfWork, ITokenService tokenService, ISettingService settingService)
        {
            _unitOfWork = unitOfWork;
            _tokenService = tokenService;
            _settingService = settingService;
        }

        public async Task<bool> Handle(ConfirmPassengerCommand request, CancellationToken cancellationToken)
        {
            var trip = await _unitOfWork.TripRepository.GetByIdAsync(request.TripId);

            if (trip == null)
            {
                throw new NotFoundException(nameof(Trip), request.TripId);
            }

            if (trip.Status != TripStatus.PENDING)
            {
                throw new BadRequestException("The trip is invalid.");
            }

            if (request.Accept)
            {
                ClaimsPrincipal? claims = _tokenService.ValidateToken(request.Token ?? "");
                Guid.TryParse(claims!.FindFirst("id")?.Value, out Guid driverId);

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
                trip.UpdatedTime = DateTime.Now;

                await _unitOfWork.TripRepository.UpdateAsync(trip);

                // Wallet transaction
                if (trip.PaymentMethod == PaymentMethod.WALLET)
                {
                    var passenger = await _unitOfWork.UserRepository.GetUserById(trip.PassengerId.ToString());
                    if (passenger == null)
                    {
                        throw new NotFoundException(nameof(User), trip.PassengerId);
                    }

                    Guid walletOwnerId = passenger.GuardianId ?? passenger.Id;
                    var walletOwnerWallet = await _unitOfWork.WalletRepository.GetByUserIdAsync(walletOwnerId);
                    if (walletOwnerWallet == null)
                    {
                        throw new NotFoundException(nameof(Wallet), walletOwnerId);
                    }

                    if (walletOwnerWallet.Balance < trip.Price)
                    {
                        throw new BadRequestException("The wallet owner's wallet does not have enough balance.");
                    }

                    walletOwnerWallet.Balance -= trip.Price;
                    walletOwnerWallet.UpdatedTime = DateTime.Now;
                    await _unitOfWork.WalletRepository.UpdateAsync(walletOwnerWallet);
                }

                await _unitOfWork.Save();

                KeyValueStore.Instance.Set($"TripConfirmationTask_{trip.Id}", "true");
            }
            else
            {
                KeyValueStore.Instance.Set($"TripConfirmationTask_{trip.Id}", "false");

            }

            return true;
        }
    }
}
