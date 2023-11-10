using Domain.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface IDriverDocumentRepository : IBaseRepository<Driverdocument>
    {
        List<Driverdocument> GetByUserId(Guid userId);
    }
}
