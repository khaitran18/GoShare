using Application.Common.Exceptions;
using Domain.DataModels;
using Domain.Interfaces;
using Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class WallettransactionRepository : BaseRepository<Wallettransaction>, IWallettransactionRepository
    {
        private readonly GoShareContext _context;
        public WallettransactionRepository(GoShareContext context) : base(context)
        {
            _context = context;
        }

        public Task<Wallettransaction> GetByIdAsync(Guid id)
        {
            Wallettransaction? w = _context.Wallettransactions.FirstOrDefault(w => w.Id.CompareTo(id) == 0);
            if (w is null) throw new NotFoundException("Transaction does not exist");
            return Task.FromResult(w);
        }

        public Task<List<Wallettransaction>> GetListByWalletId(Guid id)
        {
            return Task.FromResult(_context.Wallettransactions.Where(t => t.WalletId.CompareTo(id) == 0).ToList());
        }
    }
}
