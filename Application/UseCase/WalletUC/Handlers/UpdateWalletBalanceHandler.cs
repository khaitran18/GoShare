using Application.Common.Exceptions;
using Application.Common.Utilities;
using Application.UseCase.WalletUC.Commands;
using AutoMapper;
using Domain.DataModels;
using Domain.Enumerations;
using Domain.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCase.WalletUC.Handlers
{
    public class UpdateWalletBalanceHandler : IRequestHandler<UpdateWalletBalanceCommand, double>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public UpdateWalletBalanceHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<double> Handle(UpdateWalletBalanceCommand request, CancellationToken cancellationToken)
        {
            var wallet = await _unitOfWork.WalletRepository.GetByUserIdAsync(request.UserId);
            if (wallet == null)
            {
                throw new NotFoundException(nameof(Wallet), request.UserId);
            }

            if (request.Balance.HasValue)
            {
                var oldBalance = wallet.Balance;
                wallet.Balance = request.Balance.Value;
                wallet.UpdatedTime = DateTimeUtilities.GetDateTimeVnNow();

                // Create a new wallet transaction
                var transaction = new Wallettransaction
                {
                    Id = Guid.NewGuid(),
                    WalletId = wallet.Id,
                    Amount = request.Balance.Value - oldBalance,
                    PaymentMethod = PaymentMethod.WALLET,
                    Status = WalletTransactionStatus.SUCCESSFULL,
                    Type = WalletTransactionType.ADMIN_MODIFICATION,
                    CreateTime = DateTimeUtilities.GetDateTimeVnNow(),
                    UpdatedTime = DateTimeUtilities.GetDateTimeVnNow()
                };

                await _unitOfWork.WallettransactionRepository.AddAsync(transaction);
                await _unitOfWork.WalletRepository.UpdateAsync(wallet);
                await _unitOfWork.Save();
            }

            return wallet.Balance;
        }
    }
}
