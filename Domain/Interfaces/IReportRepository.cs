using Domain.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface IReportRepository : IBaseRepository<Report>
    {
        Task<Report?> GetByTripIdAsync(Guid tripId);
    }
}
