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

        public Task<double> Handle(GetWalletBalanceQuery request, CancellationToken cancellationToken)
        {
            double response;
            var w = _unitOfWork.WalletRepository.GetByUserIdAsync((Guid)_claims.id!).Result;
            if (w is not null) response = w.Balance;
            else throw new BadRequestException("User wallet is not found, please contact our support");
            return Task.FromResult(response);
        }
    }
}
