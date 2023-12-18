using Domain.DataModels;
using Domain.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface ITripRepository : IBaseRepository<Trip>
    {
        Task<Trip?> GetByIdAsync(Guid id);
        Task<Trip?> GetOngoingTripByPassengerId(Guid passengerId);
        Task<List<Trip>> GetTripHistoryByPassengerId(Guid userId);
        Task<List<Trip>> GetTripHistoryByBookerId(Guid userId);
        Task<Trip?> GetCurrentTripByUserId(Guid id);
        Task<(List<Trip>, int)> GetTrips(TripStatus? status, PaymentMethod? paymentMethod, TripType? type, string? sortBy, int page, int pageSize);
        Task<List<Trip>> GetPastCompletedTripsByPassengerIdAsync(Guid passengerId);
        Task<List<Trip>> GetTripHistoryByDriverId(Guid driverId);
    }
}
