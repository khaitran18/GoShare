using Domain.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface IWalletRepository : IBaseRepository<Wallet>
    {
        Task<Wallet?> GetByUserIdAsync(Guid userId);
        Task<Wallet?> GetSystemWalletAsync();
        Wallet GetById(Guid walletId);
    }
}
