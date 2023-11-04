using Domain.DataModels;
using Domain.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface ILocationRepository : IBaseRepository<Location>
    {
        Task<Location?> GetByIdAsync(Guid id);
        Task<Location?> GetByUserIdAndTypeAsync(Guid userId, LocationType type);
        Task<Location?> GetByUserIdAndLatLongAsync(Guid userId, decimal latitude, decimal longitude);
    }
}
