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
        private readonly postgresContext _context;
        private readonly IMapper _mapper;

        public UserRepository(postgresContext context, IMapper mapper) : base(context)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<List<User>> GetActiveDriversWithinRadius(ILocationRepository origin, double radius)
        {
            var result = new List<User>();
            var users = await _context.Users.ToListAsync();

            foreach (User user in users)
            {
                if (user.IsDriver && user.Status == UserStatus.ACTIVE)
                {
                    double distance = GetDistance(user.Location, origin);
                    if (distance <= radius)
                    {
                        result.Add(user);
                    }
                }
            }
            return result;
        }

        private double GetDistance(Location loc1, Location loc2)
        {
            const double R = 6371; //km
            double lat1 = loc1.Latitude * Math.PI / 180;
            double lat2 = loc2.Latitude * Math.PI / 180;
            double lon1 = loc1.Longitude * Math.PI / 180;
            double lon2 = loc2.Longitude * Math.PI / 180;
            double dLat = lat2 - lat1;
            double dLon = lon2 - lon1;
            double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) + Math.Cos(lat1) * Math.Cos(lat2) * Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            double d = R * c;
            return d;
        }
    }
}
