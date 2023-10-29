using Application.Services.Interfaces;
using Domain.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class SettingService : ISettingService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private Dictionary<string, double> _settings = null!;

        public SettingService(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
        }

        public async Task LoadSettings()
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                var settings = await unitOfWork.SettingRepository.GetAllAsync();
                _settings = settings.ToDictionary(s => s.Key, s => s.Value);
            }
        }

        public double GetSetting(string key)
        {
            return _settings[key];
        }
    }
}
