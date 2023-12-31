﻿using Application.Common.Exceptions;
using Domain.DataModels;
using Domain.Enumerations;
using Domain.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class WalletRepository : BaseRepository<Wallet>, IWalletRepository
    {
        private readonly GoShareContext _context;
        public WalletRepository(GoShareContext context) : base(context)
        {
            _context = context;
        }

        public Wallet GetById(Guid walletId)
        {
            Wallet? wallet = _context.Wallets.FirstOrDefault(w => w.Id.CompareTo(walletId) == 0);
            if (wallet is null) throw new NotFoundException("Wallet is not found");
            return wallet;
        }

        public async Task<Wallet?> GetByUserIdAsync(Guid userId)
        {
            return await _context.Wallets
                .FirstOrDefaultAsync(w => w.UserId == userId);
        }

        public async Task<Wallet?> GetSystemWalletAsync()
        {
            return await _context.Wallets
                .FirstOrDefaultAsync(w => w.Type == WalletStatus.SYSTEM);
        }
    }
}
