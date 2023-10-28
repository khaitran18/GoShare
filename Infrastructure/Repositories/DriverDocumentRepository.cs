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
        public DriverDocumentRepository(GoShareContext context) : base(context)
        {
        }
    }
}
