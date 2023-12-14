using Application.Common.Exceptions;
using Application.Common.Utilities;
using Application.UseCase.WalletUC.Commands;
using AutoMapper;
using Domain.DataModels;
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
                wallet.Balance = request.Balance.Value;
                wallet.UpdatedTime = DateTimeUtilities.GetDateTimeVnNow();
            }

            await _unitOfWork.WalletRepository.UpdateAsync(wallet);
            await _unitOfWork.Save();

            return wallet.Balance;
        }
    }
}
