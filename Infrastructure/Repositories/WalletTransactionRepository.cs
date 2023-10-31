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
    }
}
