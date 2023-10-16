﻿using AutoMapper;
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
                .ToListAsync();

            foreach (User user in users)
            {
                if (user.Isdriver && user.Status == UserStatus.ACTIVE)
                {
                    var currentLocation = user.Locations.FirstOrDefault(l => l.Type == LocationType.CURRENT_LOCATION);
                    if (currentLocation != null)
                    {
                        double distance = GetDistance(currentLocation, origin);
                        if (distance <= radius)
                        {
                            result.Add(user);
                        }
                    }
                }
            }
            return result;
        }

        private double GetDistance(Location loc1, Location loc2)
        {
            const double R = 6371; //km
            double lat1 = (double)loc1.Latitude * Math.PI / 180;
            double lat2 = (double)loc2.Latitude * Math.PI / 180;
            double lon1 = (double)loc1.Longtitude * Math.PI / 180;
            double lon2 = (double)loc2.Longtitude * Math.PI / 180;
            double dLat = lat2 - lat1;
            double dLon = lon2 - lon1;
            double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) + Math.Cos(lat1) * Math.Cos(lat2) * Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            double d = R * c;
            return d;

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
