using Application.Services.Interfaces;
using Domain.DataModels;
using Domain.Enumerations;
using Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class DriverService : IDriverService
    {
        public async Task<double> GetDriverDailyIncome(IUnitOfWork unitOfWork, Guid guid)
        {
            List<Wallettransaction> list = await unitOfWork.WallettransactionRepository.GetListByDay(guid);
            double result = 0;
            foreach (var item in list)
            {
                if (item.Type.Equals(WalletTransactionType.DRIVER_WAGE)) result += Math.Abs(item.Amount);
            }
            return result;
        }

        public async Task<(double rating,int ratingnum)> GetDriverRating(IUnitOfWork unitOfWork, Guid guid)
        {
            List<Rating> list = await unitOfWork.RatingRepository.GetListByRatee(guid);
            if (list.Count==0) return (0,0);
            else return (list.Average(r => r.RatingValue),list.Count);
        }
    }
}
