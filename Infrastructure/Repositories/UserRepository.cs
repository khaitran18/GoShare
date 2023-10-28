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
        private readonly IMapper _mapper;

        public UserRepository(GoShareContext context, IMapper mapper) : base(context)
        {
            _context = context;
            _mapper = mapper;
        }


        public async Task<List<User>> GetActiveDriversWithinRadius(Location origin, double radius)
        {
            var result = new List<User>();
            var users = await _context.Users
                .Include(u => u.Locations)
                .Include(u => u.Car)
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

        public Task<User?> GetUserById(string id)
        {
            return Task.FromResult(_context.Users.FirstOrDefault(u => u.Id.Equals(new Guid(id))));
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
            return Task.FromResult(_context.Users.FirstOrDefault(u => u.Phone.Equals(phone))!.OtpExpiryTime);
        }

        public Task<string?> GetUserRefreshTokenByUserId(string userId)
        {
            return Task.FromResult(_context.Users.FirstOrDefault(u => u.Id.Equals(new Guid(userId)))!.RefreshToken);
        }

        public Task<DateTime?> GetUserRefreshTokenExpiryTimeByUserId(string userId)
        {
            return Task.FromResult(_context.Users.FirstOrDefault(u => u.Id.Equals(new Guid(userId)))!.RefreshTokenExpiryTime);
        }

        public Task<bool> PhoneExist(string phone)
        {
            return Task.FromResult(_context.Users.Any(u => u.Phone.Equals(phone)));
        }
    }
}
