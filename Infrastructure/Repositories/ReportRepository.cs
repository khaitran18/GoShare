using Domain.DataModels;
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
    }
}
