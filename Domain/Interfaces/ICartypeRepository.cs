using Domain.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface ICartypeRepository : IBaseRepository<Cartype>
    {
        Task<double> CalculatePriceForCarType(Guid cartypeId, double distance);
    }
}
