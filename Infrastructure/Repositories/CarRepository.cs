using Domain.DataModels;
using Domain.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class CarRepository : BaseRepository<Car>, ICarRepository
    {
        private readonly GoShareContext _context;
        public CarRepository(GoShareContext context) : base(context)
        {
            _context = context;
        }

        public async Task<Car?> GetByUserId(Guid userId)
        {
            return await _context.Cars
                .FirstOrDefaultAsync(car => car.UserId == userId);
        }
    }
}
