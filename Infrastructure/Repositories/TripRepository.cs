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

        public async Task<Trip?> GetCurrentTripByUserId(Guid id)
        {
            var list = await _context.Trips.Where(t => t.PassengerId == id || t.DriverId==id).ToListAsync();
            return list.FirstOrDefault(t => t.Status != TripStatus.COMPLETED &&
                                          t.Status != TripStatus.CANCELED &&
                                          t.Status != TripStatus.TIMEDOUT);
        }

        public async Task<Trip?> GetOngoingTripByPassengerId(Guid passengerId)
        {
            return await _context.Trips
                .FirstOrDefaultAsync(t => t.PassengerId == passengerId &&
                                          t.Status != TripStatus.COMPLETED &&
                                          t.Status != TripStatus.CANCELED &&
                                          t.Status != TripStatus.TIMEDOUT);
        }

        public async Task<List<Trip>> GetTripHistoryByUserId(Guid userId)
        {
            var ongoingTrips = await _context.Trips
                .Where(t => t.PassengerId == userId &&
                            (t.Status == TripStatus.GOING_TO_PICKUP || t.Status == TripStatus.GOING))
                .Include(t => t.StartLocation)
                .Include(t => t.Driver)
                .Include(t => t.EndLocation)
                .Include(t => t.Driver!.Car)
                .Include(t => t.Cartype)
                .Include(t => t.Booker)
                .OrderByDescending(t => t.CreateTime)
                .ToListAsync();

            var completedTrips = await _context.Trips
                .Where(t => t.PassengerId == userId && t.Status == TripStatus.COMPLETED)
                .Include(t => t.StartLocation)
                .Include(t => t.Driver)
                .Include(t => t.EndLocation)
                .Include(t => t.Driver!.Car)
                .Include(t => t.Cartype)
                .Include(t => t.Booker)
                .OrderByDescending(t => t.CreateTime)
                .ToListAsync();

            var combinedTrips = ongoingTrips.Concat(completedTrips).OrderByDescending(t => t.CreateTime).ToList();

            return combinedTrips;
        }
    }
}
