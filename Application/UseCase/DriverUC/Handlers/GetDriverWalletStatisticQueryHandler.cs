using Application.Common.Dtos;
using Application.Common.Utilities;
using Application.UseCase.DriverUC.Queries;
using AutoMapper;
using Domain.Enumerations;
using Domain.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCase.DriverUC.Handlers
{
    public class GetDriverWalletStatisticQueryHandler : IRequestHandler<GetDriverWalletStatisticQuery, List<WalletMonthStatistic>>
    {
        private readonly UserClaims _claims;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetDriverWalletStatisticQueryHandler(UserClaims claims, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _claims = claims;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<List<WalletMonthStatistic>> Handle(GetDriverWalletStatisticQuery request, CancellationToken cancellationToken)
        {
            var response = new List<WalletMonthStatistic>();
            Guid walletId = (await _unitOfWork.WalletRepository.GetByUserIdAsync((Guid)_claims.id!))!.Id;
            var transactions = await _unitOfWork.WallettransactionRepository.GetListByWalletId(walletId);
            transactions = transactions
                .Where(u=>u.Type.Equals(WalletTransactionType.PASSENGER_REFUND)
                || u.Type.Equals(WalletTransactionType.TOPUP)
                || u.Type.Equals(WalletTransactionType.DRIVER_WAGE))
                .ToList();
            int numberOfMonth = 6;
            int numOfWeekInMonth = 4;
            for (int i = numberOfMonth; i >= 0; i--)
            {
                var t = DateTimeUtilities.GetDateTimeVnNow().AddMonths((-1) * i);
                var monthTransactions = transactions.Where(u => u.CreateTime.Month == t.Month && u.Status.Equals(WalletTransactionStatus.SUCCESSFULL));
                WalletMonthStatistic stat = new WalletMonthStatistic();
                stat.Month = t.Month;
                stat.MonthTotal = monthTransactions.Sum(u => Math.Abs(u.Amount));
                stat.WeekAverage = stat.MonthTotal / numOfWeekInMonth;
                if (response.Count != 0 && response.Last().MonthTotal != 0) 
                    stat.CompareToLastMonth = monthTransactions.Sum(u => Math.Abs(u.Amount)) / response.Last().MonthTotal;
                if (i != numberOfMonth) response.Add(stat);
            }
            return response;
        }
    }
}
