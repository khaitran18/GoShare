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
    public class LocationRepository : BaseRepository<Location>, ILocationRepository
    {
        private readonly GoShareContext _context;
        public LocationRepository(GoShareContext context) : base(context)
        {
            _context = context;
        }

        public async Task<Location?> GetByIdAsync(Guid id)
        {
            return await _context.Locations.FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task<Location?> GetByUserIdAndLatLongAsync(Guid userId, decimal latitude, decimal longitude)
        {
            return await _context.Locations.FirstOrDefaultAsync(l => l.UserId == userId && l.Latitude == latitude && l.Longtitude == longitude);
        }

        public async Task<Location?> GetByUserIdAndTypeAsync(Guid userId, LocationType type)
        {
            return await _context.Locations.FirstOrDefaultAsync(l => l.UserId == userId && l.Type == type);
        }
    }
}
