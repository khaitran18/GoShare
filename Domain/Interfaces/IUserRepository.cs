using Domain.DataModels;

namespace Domain.Interfaces
{
    public interface IUserRepository:IBaseRepository<User>
    {
        Task<List<User>> GetActiveDriversWithinRadius(Location origin, double radius);
    }
}
