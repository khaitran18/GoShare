using Application.Common.Dtos;
using Application.Common.Exceptions;
using Application.UseCase.WallettransactionUC.Queries;
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

namespace Application.UseCase.WallettransactionUC.Handlers
{
    public class GetSystemTransactionHandler : IRequestHandler<GetSystemTransactionQuery, List<WalletTransactionDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetSystemTransactionHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<List<WalletTransactionDto>> Handle(GetSystemTransactionQuery request, CancellationToken cancellationToken)
        {
            var systemWallet = await _unitOfWork.WalletRepository.GetSystemWalletAsync();
            if (systemWallet == null)
            {
                throw new NotFoundException(nameof(Wallet), WalletStatus.SYSTEM);
            }

            var systemTransactions = await _unitOfWork.WallettransactionRepository.GetListByWalletId(systemWallet.Id);

            var systemTransactionDtos = _mapper.Map<List<WalletTransactionDto>>(systemTransactions);

            return systemTransactionDtos;
        }
    }
}
