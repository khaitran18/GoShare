using Application.Common.Dtos;
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

namespace Application.Queries.Handler
{
    public class GetUserTransactionQueryHandler : IRequestHandler<GetUserTransactionQuery, List<WalletTransactionDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserClaims _claims;
        private readonly IMapper _mapper;

        public GetUserTransactionQueryHandler(IUnitOfWork unitOfWork, UserClaims claims, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _claims = claims;
            _mapper = mapper;
        }

        public Task<List<WalletTransactionDto>> Handle(GetUserTransactionQuery request, CancellationToken cancellationToken)
        {
            var response = new List<WalletTransactionDto>();
            Wallet? w = _unitOfWork.WalletRepository.GetByUserIdAsync((Guid)_claims.id!).Result;
            if (w is null) throw new DirectoryNotFoundException("User wallet is not found! Please contact our support");
            //get wallet transactions
            List<Wallettransaction> transactions = _unitOfWork.WallettransactionRepository.GetListByWalletId(w.Id).Result;
            //get transaction that is success or failed
            transactions = transactions.Where(t => !t.Status.Equals(WalletTransactionStatus.PENDING)).ToList();
            return Task.FromResult(_mapper.Map<List<WalletTransactionDto>>(transactions));
        }
    }
}
