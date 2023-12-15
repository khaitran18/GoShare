using Domain.DataModels;
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
    public class SettingRepository : BaseRepository<Setting>, ISettingRepository
    {
        private readonly GoShareContext _context;
        public SettingRepository(GoShareContext context) : base(context)
        {
            _context = context;
        }

        public async Task<Setting?> GetSettingById(Guid id)
        {
            return await _context.Settings.FirstOrDefaultAsync(s => s.Id == id);
        }
    }
}
