using Domain.DataModels;
using Domain.Enumerations;
using Domain.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class ReportRepository : BaseRepository<Report>, IReportRepository
    {
        private readonly GoShareContext _context;
        public ReportRepository(GoShareContext context) : base(context)
        {
            _context = context;
        }

        public async Task<Report?> GetByTripIdAsync(Guid tripId)
        {
            return await _context.Reports.FirstOrDefaultAsync(r => r.TripId == tripId);
        }

        public async Task<Report?> GetByIdAsync(Guid id)
        {
            return await _context.Reports
                .Include(r => r.Trip)
                    .ThenInclude(t => t.TripImages)
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<(List<Report>, int)> GetReports(ReportStatus? status, int page, int pageSize)
        {
            IQueryable<Report> query = _context.Reports
                .Include(r => r.Trip)
                .AsQueryable();

            // Filter by status
            if (status.HasValue)
            {
                query = query.Where(r => r.Status == status);
            }

            // Order by newest to oldest
            query = query.OrderByDescending(r => r.CreateTime);

            var totalCount = await query.CountAsync();

            // Pagination
            query = query.Skip((page - 1) * pageSize).Take(pageSize);

            var reports = await query.ToListAsync();

            return (reports, totalCount);
        }
    }
}
