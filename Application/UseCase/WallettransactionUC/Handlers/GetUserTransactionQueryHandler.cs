using Application.Common.Dtos;
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
    public class GetUserTransactionQueryHandler : IRequestHandler<GetUserTransactionQuery, PaginatedResult<WalletTransactionDto>>
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

        public async Task<PaginatedResult<WalletTransactionDto>> Handle(GetUserTransactionQuery request, CancellationToken cancellationToken)
        {
            var response = new List<WalletTransactionDto>();

            Wallet? w;
            if (request.UserId.HasValue)
            {
                // Admin is querying
                w = await _unitOfWork.WalletRepository.GetByUserIdAsync(request.UserId.Value);
            }
            else
            {
                // User is querying their own transactions
                w = await _unitOfWork.WalletRepository.GetByUserIdAsync((Guid)_claims.id!);
            }

            if (w is null) throw new DirectoryNotFoundException("User wallet is not found! Please contact our support");
            //get wallet transactions
            var transactions = _unitOfWork.WallettransactionRepository.GetListByWalletId(w.Id).Result.AsQueryable();

            //get transaction that is success or failed
            //transactions = transactions.Where(t => !t.Status.Equals(WalletTransactionStatus.PENDING)).ToList();

            //Get transaction for driver
            if (_claims.Role.Equals(UserRoleEnumerations.Driver))
            {
                transactions = transactions.Where(t =>
                    t.Type.Equals(WalletTransactionType.TOPUP)
                    || t.Type.Equals(WalletTransactionType.DRIVER_WAGE));
            }
            //Get transaction for user
            else if (_claims.Role.Equals(UserRoleEnumerations.User))
            {
                transactions = transactions.Where(t =>
                t.Type.Equals(WalletTransactionType.TOPUP)
                || t.Type.Equals(WalletTransactionType.PASSENGER_PAYMENT)
                || t.Type.Equals(WalletTransactionType.PASSENGER_REFUND));
            }

            // Sort by
            if (!string.IsNullOrEmpty(request.SortBy))
            {
                switch (request.SortBy.ToLower())
                {
                    case "topup":
                        transactions = transactions.OrderByDescending(u => u.Type.Equals(WalletTransactionType.TOPUP));
                        break;
                    case "wage":
                        transactions = transactions.OrderByDescending(u => u.Type.Equals(WalletTransactionType.DRIVER_WAGE));
                        break;
                    case "payment":
                        transactions = transactions.OrderByDescending(u => u.Type.Equals(WalletTransactionType.PASSENGER_PAYMENT));
                        break;
                    case "cash":
                        transactions = transactions.OrderByDescending(u => u.PaymentMethod.Equals(PaymentMethod.CASH));
                        break;
                    case "vnpay":
                        transactions = transactions.OrderByDescending(u => u.PaymentMethod.Equals(PaymentMethod.VNPAY));
                        break;
                    case "wallet":
                        transactions = transactions.OrderByDescending(u => u.PaymentMethod.Equals(PaymentMethod.WALLET));
                        break;
                    default:
                        transactions = transactions.OrderByDescending(u => u.CreateTime);
                        break;
                }
            }

            var totalCount = transactions.Count();

            // Pagination
            transactions = transactions.Skip((request.Page - 1) * request.PageSize).Take(request.PageSize);

            var list = transactions.ToList();
            response = _mapper.Map<List<WalletTransactionDto>>(transactions);
            var paginatedResult = new PaginatedResult<WalletTransactionDto>(
                response,
                totalCount,
                request.Page,
                request.PageSize
            );
            return paginatedResult;
        }
    }
}
