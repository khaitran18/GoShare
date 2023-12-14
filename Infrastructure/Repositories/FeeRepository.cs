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
    public class FeeRepository : BaseRepository<Fee>, IFeeRepository
    {
        private readonly GoShareContext _context;
        public FeeRepository(GoShareContext context) : base(context)
        {
            _context = context;
        }

        public Task<List<Fee>> GetAllWithPoliciesAsync()
        {
            return _context.Fees
            .Include(f => f.Feepolicies)
            .SelectMany(f => f.Feepolicies, (f, p) => new { Fee = f, Policy = p })
            .OrderBy(fp => fp.Fee.CarType)
            .ThenBy(fp => fp.Policy.MinDistance)
            .Select(fp => fp.Fee)
            .Distinct()
            .ToListAsync();
        }
    }
}
