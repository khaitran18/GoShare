﻿using Domain.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface IWallettransactionRepository : IBaseRepository<Wallettransaction>
    {
        Task<Wallettransaction> GetByIdAsync(Guid id);
        Task<List<Wallettransaction>> GetListByWalletId(Guid id);
        Task<List<Wallettransaction>> GetListByDay(Guid guid);
    }
}
