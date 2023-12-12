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
        Task<bool> IsVerified(Guid id);
        Task<(List<User>, int)> GetDependents(Guid guardianId, string? sortBy, int page, int pageSize);
        Task<(List<User>, int)> GetDriverAsync(int page, int pageSize, string? sortBy);
        Task<List<User>> GetDependentsByGuardianId(Guid userId);
        Task<(List<User>, int)> GetUsersAsync(int page, int pageSize, string? sortBy);
        Task<bool> IsDependent(Guid UserId);
        Task<bool> IsBanned(Guid userId, out string? reason);
        Task<List<User>> GetDriversWithDebt();
        Task<List<User>> GetDriversWarnedRating();
    }
}
