using Application.Common.Dtos;
using Application.Common.Exceptions;
using Domain.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Queries.Handler
{
    public class GetWalletBalanceQueryHandler : IRequestHandler<GetWalletBalanceQuery, double>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserClaims _claims;

        public GetWalletBalanceQueryHandler(IUnitOfWork unitOfWork, UserClaims claims)
        {
            _unitOfWork = unitOfWork;
            _claims = claims;
        }

        public async Task<double> Handle(GetWalletBalanceQuery request, CancellationToken cancellationToken)
        {
            double response;
            var w = _unitOfWork.WalletRepository.GetByUserIdAsync((Guid)_claims.id!).Result;
            // if the user has a wallet
            if (w is not null) response = w.Balance;
            else {
                var u = await _unitOfWork.UserRepository.GetUserById(_claims.ToString()!);
                // if user is not a guardian
                if (u?.GuardianId is null) throw new BadRequestException("User wallet is not found, please contact our support");
                else
                {
                    var gWallet = await _unitOfWork.WalletRepository.GetByUserIdAsync((Guid)u.GuardianId);
                    if (gWallet is not null) response = gWallet.Balance;
                    else throw new BadRequestException("User wallet is not found, please contact our support");
                }
            }
            return response;
        }
    }
}
