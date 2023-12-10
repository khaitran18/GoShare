using Domain.DataModels;
using Domain.Enumerations;
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
        Task<Report?> GetByIdAsync(Guid id);
        Task<(List<Report>, int)> GetReports(ReportStatus? status, int page, int pageSize);
    }
}
