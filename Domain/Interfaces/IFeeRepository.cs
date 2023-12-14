using Domain.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface IFeeRepository : IBaseRepository<Fee>
    {
        Task<List<Fee>> GetAllWithPoliciesAsync();
    }
}
