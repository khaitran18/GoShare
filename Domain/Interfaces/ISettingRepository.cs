using Domain.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface ISettingRepository : IBaseRepository<Setting>
    {
        Task<Setting?> GetSettingById(Guid id);
    }
}
