using Application.Common.Exceptions;
using Domain.DataModels;
using Domain.Enumerations;
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

        public async Task<bool> CarDupplicated(Guid id)
        {
            return await Task.FromResult(_context.Cars.Any(a => a.UserId.CompareTo(id) == 0));
        }

        public async Task<Car?> GetByUserId(Guid userId)
        {
            return await _context.Cars
                .FirstOrDefaultAsync(car => car.UserId == userId);
        }

        public async Task<bool> VerifyCar(Guid id, DateTime verifiedTo)
        {
            Car? car = _context.Cars.FirstOrDefault(c => c.UserId.CompareTo(id) == 0);
            if (car is not null)
            {
                car.Status = (short)CarStatusEnumerations.Verified;
                car.VerifiedTo = verifiedTo;
                await _context.SaveChangesAsync();
            }
            else throw new BadRequestException("User's car is not found");
            return true;
        }
    }
}
