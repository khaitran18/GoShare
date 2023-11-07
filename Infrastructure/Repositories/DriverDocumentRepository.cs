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
    public class DriverDocumentRepository : BaseRepository<Driverdocument>, IDriverDocumentRepository
    {
        private readonly GoShareContext _context;
        public DriverDocumentRepository(GoShareContext context) : base(context)
        {
            _context = context;
        }

        public List<Driverdocument> GetByUserId(Guid userId) { 
            Guid id = _context.Cars.FirstOrDefault(c=>c.UserId.Equals(userId))!.Id;
            return _context.Driverdocuments.Where(d => d.CarId.Equals(id)).ToList();
        }
    }
}
