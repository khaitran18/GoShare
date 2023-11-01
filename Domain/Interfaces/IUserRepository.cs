using Domain.DataModels;

namespace Domain.Interfaces
{
    public interface IUserRepository : IBaseRepository<User>
    {
        Task<List<User>> GetActiveDriversWithinRadius(Location origin, double radius);
        Task<User?> GetUserByPhone(string phone);
        Task<string?> GetUserOtpByPhone(string phone);
        Task<DateTime> GetUserOtpExpiryTimeByPhone(string phone);
        Task<bool> PhoneExist(string phone);
        Task<string?> GetUserRefreshTokenByUserId(string userId);
        Task<DateTime?> GetUserRefreshTokenExpiryTimeByUserId(string userId);
        Task<User?> GetUserById(string id);
        Task<bool> VerifyDriver(Guid userGuid);
    }
}
