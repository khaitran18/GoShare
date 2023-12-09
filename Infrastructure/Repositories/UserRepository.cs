using Application.Common.Exceptions;
using Application.Common.Utilities;
using AutoMapper;
using Domain.DataModels;
using Domain.Enumerations;
using Domain.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class UserRepository : BaseRepository<User>, IUserRepository
    {
        private readonly GoShareContext _context;

        public UserRepository(GoShareContext context) : base(context)
        {
            _context = context;
        }

        public async Task<List<User>> GetActiveDriversWithinRadius(Location origin, double radius)
        {
            var result = new List<User>();
            var users = await _context.Users
                .Include(u => u.Locations)
                .Include(u => u.Car)
                .AsNoTracking() // Disable entity tracking for getting updated driver location
                .ToListAsync();

            foreach (User user in users)
            {
                if (user.Isdriver && user.Status == UserStatus.ACTIVE)
                {
                    var currentLocation = user.Locations.FirstOrDefault(l => l.Type == LocationType.CURRENT_LOCATION);
                    if (currentLocation != null)
                    {
                        double distance = MapsUtilities.GetDistance(currentLocation, origin);
                        if (distance <= radius)
                        {
                            result.Add(user);
                        }
                    }
                }
            }
            return result;
        }

        public async Task<(List<User>, int)> GetDependents(Guid guardianId, string? sortBy, int page, int pageSize)
        {
            IQueryable<User> query = _context.Users.Where(u => u.GuardianId == guardianId).AsQueryable();

            // Sort by
            if (!string.IsNullOrEmpty(sortBy))
            {
                switch (sortBy.ToLower())
                {
                    case "name":
                        query = query.OrderBy(u => u.Name);
                        break;
                    case "name_desc":
                        query = query.OrderByDescending(u => u.Name);
                        break;
                }
            }

            var totalCount = await query.CountAsync();

            // Pagination
            query = query.Skip((page - 1) * pageSize).Take(pageSize);

            var dependents = await query.ToListAsync();

            return (dependents, totalCount);
        }

        public async Task<List<User>> GetDependentsByGuardianId(Guid userId)
        {
            return await _context.Users
                .Where(u => u.GuardianId == userId)
                .ToListAsync();
        }

        public async Task<(List<User>, int)> GetDriverAsync(int page, int pageSize, string? sortBy)
        {
            IQueryable<User> drivers = _context.Users
                .Include(u=>u.Car)
                .Where(u => u.Isdriver.Equals(true))
                .AsQueryable();
            IQueryable<User> users = _context.Users
                .Include(u => u.Car)
                .Where(u => u.Isdriver.Equals(false))
                .Where(u => !u.Car.Equals(null))
//                .Where(u => u.Car!.Status.Equals(CarStatusEnumerations.Not_Verified))
                .AsQueryable();
            var query = drivers.Concat(users);

            // Sort by
            if (!string.IsNullOrEmpty(sortBy))
            {
                switch (sortBy.ToLower())
                {
                    case "name":
                        query = query.OrderBy(u => u.Name);
                        break;
                    case "name_desc":
                        query = query.OrderByDescending(u => u.Name);
                        break;
                    case "verify":
                        query = query.OrderBy(u => u.Car!.VerifiedTo);
                        break;
                    case "verify_desc":
                        query = query.OrderByDescending(u => u.Car!.VerifiedTo);
                        break;
                    case "update":
                        query = query.OrderByDescending(u => u.UpdatedTime);
                        break;
                    case "update_asc":
                        query = query.OrderBy(u => u.UpdatedTime);
                        break;
                    case "disabled":
                        query = query.OrderBy(u => u.Status.Equals(UserStatus.BANNED));
                        break;
                    default:
                        query = query.OrderByDescending(u => u.CreateTime);
                        break;
                }
            }
            var totalCount = await query.CountAsync();

            // Pagination
            query = query.Skip((page - 1) * pageSize).Take(pageSize);

            var list = await query.ToListAsync();

            return (list, totalCount);
        }

        public async Task<User?> GetUserById(string id)
        {

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Id.Equals(new Guid(id)));

            if (user != null && user.Isdriver)
            {
                user = await _context.Users
                    .Include(u => u.Car)
                    .FirstOrDefaultAsync(u => u.Id.Equals(new Guid(id)));
            }
            if (user != null && user.GuardianId != null)
            {
                user = await _context.Users
                    .Include(u => u.Guardian)
                    .FirstOrDefaultAsync(u => u.Id.Equals(new Guid(id)));
            }

            return user;
        }

        public Task<User?> GetUserByPhone(string phone)
        {
            return Task.FromResult(_context.Users.FirstOrDefault(u => u.Phone.Equals(phone)));
        }

        public Task<string?> GetUserOtpByPhone(string phone)
        {
            return Task.FromResult(_context.Users.FirstOrDefault(u => u.Phone.Equals(phone))?.Otp);
        }

        public Task<DateTime> GetUserOtpExpiryTimeByPhone(string phone)
        {
            return Task.FromResult((DateTime)_context.Users.FirstOrDefault(u => u.Phone.Equals(phone))!.OtpExpiryTime!);
        }

        public Task<string?> GetUserRefreshTokenByUserId(string userId)
        {
            return Task.FromResult(_context.Users.FirstOrDefault(u => u.Id.Equals(new Guid(userId)))!.RefreshToken);
        }

        public Task<DateTime?> GetUserRefreshTokenExpiryTimeByUserId(string userId)
        {
            return Task.FromResult(_context.Users.FirstOrDefault(u => u.Id.Equals(new Guid(userId)))!.RefreshTokenExpiryTime);
        }

        public async Task<(List<User>, int)> GetUsersAsync(int page, int pageSize, string? sortBy)
        {
            IQueryable<User> query = _context.Users.Include(u=>u.Guardian);

            // Sort by
            if (!string.IsNullOrEmpty(sortBy))
            {
                switch (sortBy.ToLower())
                {
                    case "name":
                        query = query.OrderBy(u => u.Name);
                        break;
                    case "name_desc":
                        query = query.OrderByDescending(u => u.Name);
                        break;
                    case "create_asc":
                        query = query.OrderBy(u => u.CreateTime);
                        break;
                    case "create":
                        query = query.OrderByDescending(u => u.CreateTime);
                        break;
                    case "update":
                        query = query.OrderByDescending(u => u.UpdatedTime);
                        break;
                    case "update_asc":
                        query = query.OrderBy(u => u.UpdatedTime);
                        break;
                    case "verify":
                        query = query.OrderBy(u => u.Isverify);
                        break;
                    case "verify_desc":
                        query= query.OrderByDescending(u => u.Isverify);
                        break;
                    case "disabled":
                        query = query.OrderBy(u => u.Status.Equals(UserStatus.BANNED));
                        break;
                    case "dependent":
                        query = query.OrderByDescending(u => !u.GuardianId.Equals(null));
                        break;
                    case "guardian":
                        query = query.OrderBy(u => !u.GuardianId.Equals(null));
                        break;
                    default:
                        query = query.OrderByDescending(u => u.CreateTime);
                        break;
                }
            }

            var totalCount = await query.CountAsync();

            // Pagination
            query = query.Skip((page - 1) * pageSize).Take(pageSize);

            var dependents = await query.ToListAsync();

            return (dependents, totalCount);
        }

        public Task<bool> IsBanned(Guid userId, out string? reason)
        {
            reason = _context.Users.FirstOrDefault(u => u.Id.CompareTo(userId) == 0)?.DisabledReason;
            return Task.FromResult(reason is null ? false : true);
        }

        public Task<bool> IsDependent(Guid UserId)
        {
            User? u = _context.Users.FirstOrDefault(u => u.Id.CompareTo(UserId) == 0);
            if (u is null) throw new NotFoundException("User is not found");
            else return u.GuardianId == null ? Task.FromResult(false) : Task.FromResult(true);
        }

        public Task<bool> IsVerified(Guid id)
        {
            User? u = _context.Users.FirstOrDefaultAsync(u => u.Id.CompareTo(id) == 0).Result;
            if (u is null) throw new NotFoundException("User is not found");
            return Task.FromResult(u.Isverify);
        }

        public Task<bool> PhoneExist(string phone)
        {
            return Task.FromResult(_context.Users.Any(u => u.Phone.Equals(phone)));
        }

        public async Task<bool> VerifyDriver(Guid userGuid)
        {
            User? u = _context.Users.FirstOrDefault(u => u.Id.Equals(userGuid));
            if (u is not null) 
            {
                u.Isdriver = true;
                await _context.SaveChangesAsync();
            }
            else throw new NotFoundException("User not found");
            return true;
        }
    }
}
