using Domain.DataModels;
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
        Task<List<Trip>> GetTripHistoryByUserId(Guid userId);
        Task<Trip?> GetCurrentTripByUserId(Guid id);
    }
}
