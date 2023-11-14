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
    public class TripRepository : BaseRepository<Trip>, ITripRepository
    {
        private readonly GoShareContext _context;
        public TripRepository(GoShareContext context) : base(context)
        {
            _context = context;
        }

        public async Task<Trip?> GetByIdAsync(Guid id)
        {
            return await _context.Trips
                .Include(t => t.StartLocation)
                .Include(t => t.EndLocation)
                .Include(t => t.Driver)
                .Include(t => t.Cartype)
                .Include(t => t.Passenger)
                    .ThenInclude(p => p.Guardian)
                .Include(t => t.Booker)
                .FirstOrDefaultAsync(t => t.Id == id);
        }
    }
}
