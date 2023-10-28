using Application.Common.Exceptions;
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
    public class CartypeRepository : BaseRepository<Cartype>, ICartypeRepository
    {
        private readonly GoShareContext _context;
        public CartypeRepository(GoShareContext context) : base(context)
        {
            _context = context;
        }

        public async Task<double> CalculatePriceForCarType(Guid cartypeId, double distance)
        {
            var carType = await _context.Cartypes
                .Include(c => c.Fees)
                .ThenInclude(f => f.Feepolicies)
                .FirstOrDefaultAsync(c => c.Id == cartypeId);

            if (carType == null)
            {
                throw new NotFoundException("Car type not found");
            }

            var fee = carType.Fees.FirstOrDefault();

            if (fee == null)
            {
                throw new NotFoundException("Fee not found for this car type");
            }

            double totalPrice = fee.BasePrice * fee.BaseDistance;
            double remainingDistance = distance - fee.BaseDistance;
            var sortedPolicies = fee.Feepolicies.OrderBy(p => p.MinDistance);

            foreach (var policy in sortedPolicies)
            {
                if (remainingDistance <= 0)
                {
                    break;
                }

                double policyDistance = policy.MaxDistance.HasValue 
                    ? Math.Min(remainingDistance, policy.MaxDistance.Value - policy.MinDistance) 
                    : remainingDistance;
                totalPrice += policyDistance * policy.PricePerKm;
                remainingDistance -= policyDistance;
            }

            return totalPrice;
        }
    }
}
