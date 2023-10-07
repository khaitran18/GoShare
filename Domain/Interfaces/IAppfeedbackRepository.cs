using Domain.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface IAppfeedbackRepository : IBaseRepository<Appfeedback>
    {
        Task<(List<Appfeedback>, int)> GetAppfeedbacks(string? sortBy, int page, int pageSize);
    }
}
