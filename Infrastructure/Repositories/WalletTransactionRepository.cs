using Application.Common.Exceptions;
using Application.Common.Utilities;
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

        public Task<List<Wallettransaction>> GetListByDay(Guid guid)
        {
            DateTime TodayTime = DateTimeUtilities.GetDateTimeVnNow().Date;
            DateTime CurrentDatetime = DateTimeUtilities.GetDateTimeVnNow();
            var list = _context.Wallets
                .Include(u => u.Wallettransactions)
                .FirstOrDefault(u => u.UserId.Equals(guid))!
                .Wallettransactions.Where(t => t.CreateTime.CompareTo(TodayTime) > 0
                && t.CreateTime.CompareTo(CurrentDatetime) < 0
                && t.Status.Equals(WalletTransactionStatus.SUCCESSFULL))
                .ToList();
            return Task.FromResult(list);
        }

        public Task<List<Wallettransaction>> GetListByWalletId(Guid id)
        {
            return _context.Wallettransactions
                .Where(t => t.WalletId.CompareTo(id) == 0)
                .OrderByDescending(t=>t.CreateTime)
                .ToListAsync();
        }
    }
}
