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
    public class TripImageRepository : BaseRepository<TripImage>, ITripImageRepository
    {
        private readonly GoShareContext _context;
        public TripImageRepository(GoShareContext context) : base(context)
        {
            _context = context;
        }
    }
}
