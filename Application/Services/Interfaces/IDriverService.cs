using Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services.Interfaces
{
    public interface IDriverService
    {
        Task<double> GetDriverDailyIncome(IUnitOfWork unitOfWork, Guid guid);
        Task<(double rating,int ratingnum)> GetDriverRating(IUnitOfWork unitOfWork, Guid guid);
    }
}
