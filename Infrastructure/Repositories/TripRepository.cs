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
                    .ThenInclude(d => d!.Car)
                .Include(t => t.Cartype)
                .Include(t => t.Passenger)
                    .ThenInclude(p => p.Guardian)
                .Include(t => t.Booker)
                .Include(t => t.TripImages)
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

        public async Task<List<Trip>> GetPastCompletedTripsByPassengerIdAsync(Guid passengerId)
        {
            return await _context.Trips
                .Where(t => t.PassengerId == passengerId && t.Status == TripStatus.COMPLETED)
                .ToListAsync();
        }

        public async Task<List<Trip>> GetTripHistoryByPassengerId(Guid userId)
        {
            var ongoingTrips = await _context.Trips
                .Where(t => t.PassengerId == userId &&
                            (t.Status == TripStatus.GOING_TO_PICKUP || t.Status == TripStatus.GOING))
                .Include(t => t.StartLocation)
                .Include(t => t.Driver)
                    .ThenInclude(t => t!.Car)
                .Include(t => t.Passenger)
                    .ThenInclude(p => p.Guardian)
                .Include(t => t.EndLocation)
                .Include(t => t.Cartype)
                .Include(t => t.Booker)
                .Include(t => t.TripImages)
                .OrderByDescending(t => t.CreateTime)
                .ToListAsync();

            var completedTrips = await _context.Trips
                .Where(t => t.PassengerId == userId && t.Status == TripStatus.COMPLETED)
                .Include(t => t.StartLocation)
                .Include(t => t.Driver)
                    .ThenInclude(t => t!.Car)
                .Include(t => t.Passenger)
                    .ThenInclude(p => p.Guardian)
                .Include(t => t.EndLocation)
                .Include(t => t.Cartype)
                .Include(t => t.Booker)
                .Include(t => t.TripImages)
                .OrderByDescending(t => t.CreateTime)
                .ToListAsync();

            var combinedTrips = ongoingTrips.Concat(completedTrips).OrderByDescending(t => t.CreateTime).ToList();

            return combinedTrips;
        }

        public async Task<List<Trip>> GetTripHistoryByBookerId(Guid userId)
        {
            var ongoingTrips = await _context.Trips
                .Where(t => t.BookerId == userId &&
                            (t.Status == TripStatus.GOING_TO_PICKUP || t.Status == TripStatus.GOING))
                .Include(t => t.StartLocation)
                .Include(t => t.Driver)
                    .ThenInclude(t => t!.Car)
                .Include(t => t.Passenger)
                    .ThenInclude(p => p.Guardian)
                .Include(t => t.EndLocation)
                .Include(t => t.Cartype)
                .Include(t => t.Booker)
                .Include(t => t.TripImages)
                .OrderByDescending(t => t.CreateTime)
                .ToListAsync();

            var completedTrips = await _context.Trips
                .Where(t => t.BookerId == userId && t.Status == TripStatus.COMPLETED)
                .Include(t => t.StartLocation)
                .Include(t => t.Driver)
                    .ThenInclude(t => t!.Car)
                .Include(t => t.Passenger)
                    .ThenInclude(p => p.Guardian)
                .Include(t => t.EndLocation)
                .Include(t => t.Cartype)
                .Include(t => t.Booker)
                .Include(t => t.TripImages)
                .OrderByDescending(t => t.CreateTime)
                .ToListAsync();

            var combinedTrips = ongoingTrips.Concat(completedTrips).OrderByDescending(t => t.CreateTime).ToList();

            return combinedTrips;
        }

        public async Task<List<Trip>> GetTripHistoryByDriverId(Guid driverId)
        {
            var completedTrips = await _context.Trips
                .Where(t => t.DriverId == driverId && t.Status == TripStatus.COMPLETED)
                .Include(t => t.StartLocation)
                .Include(t => t.Driver)
                    .ThenInclude(t => t!.Car)
                .Include(t => t.Passenger)
                    .ThenInclude(p => p.Guardian)
                .Include(t => t.EndLocation)
                .Include(t => t.Cartype)
                .Include(t => t.Booker)
                .Include(t => t.TripImages)
                .OrderByDescending(t => t.CreateTime)
                .ToListAsync();

            return completedTrips;
        }

        public async Task<(List<Trip>, int)> GetTrips(TripStatus? status, PaymentMethod? paymentMethod, TripType? type, string? sortBy, int page, int pageSize)
        {
            IQueryable<Trip> query = _context.Trips
                .Include(t => t.StartLocation)
                .Include(t => t.EndLocation)
                .Include(t => t.Driver)
                    .ThenInclude(d => d!.Car)
                .Include(t => t.Cartype)
                .Include(t => t.Passenger)
                    .ThenInclude(p => p.Guardian)
                .Include(t => t.Booker)
                .Include(t => t.TripImages)
                .AsQueryable();

            // Filter by status, payment method, and type
            if (status.HasValue)
            {
                query = query.Where(t => t.Status == status);
            }
            if (paymentMethod.HasValue)
            {
                query = query.Where(t => t.PaymentMethod == paymentMethod);
            }
            if (type.HasValue)
            {
                query = query.Where(t => t.Type == type);
            }

            // Pre-order by CreateTime
            query = query.OrderByDescending(t => t.CreateTime);

            // Sort by the specified field
            switch (sortBy)
            {
                case "passengerName":
                    query = query.OrderBy(t => t.Passenger.Name);
                    break;
                case "passengerName_desc":
                    query = query.OrderByDescending(t => t.Passenger.Name);
                    break;
                case "startTime":
                    query = query.OrderBy(t => t.StartTime);
                    break;
                case "startTime_desc":
                    query = query.OrderByDescending(t => t.StartTime);
                    break;
                case "endTime":
                    query = query.OrderBy(t => t.EndTime);
                    break;
                case "endTime_desc":
                    query = query.OrderByDescending(t => t.EndTime);
                    break;
                case "pickupTime":
                    query = query.OrderBy(t => t.PickupTime);
                    break;
                case "pickupTime_desc":
                    query = query.OrderByDescending(t => t.PickupTime);
                    break;
                case "distance":
                    query = query.OrderBy(t => t.Distance);
                    break;
                case "distance_desc":
                    query = query.OrderByDescending(t => t.Distance);
                    break;
                case "price":
                    query = query.OrderBy(t => t.Price);
                    break;
                case "price_desc":
                    query = query.OrderByDescending(t => t.Price);
                    break;
                case "updatedTime":
                    query = query.OrderBy(t => t.UpdatedTime);
                    break;
                case "updatedTime_desc":
                    query = query.OrderByDescending(t => t.UpdatedTime);
                    break;
            }

            var totalCount = await query.CountAsync();

            // Pagination
            query = query.Skip((page - 1) * pageSize).Take(pageSize);

            var trips = await query.ToListAsync();

            return (trips, totalCount);
        }
    }
}
