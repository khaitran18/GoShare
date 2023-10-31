using Domain.DataModels;
using Domain.Enumerations;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Data
{
    public static class DataSeeder
    {
        public static void SeedSettings(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Setting>()
                .HasData(
                    new Setting { Id = Guid.NewGuid(), Key = "FIND_DRIVER_RADIUS", Value = 1, DataUnit = SettingDataUnit.KILOMETERS },
                    new Setting { Id = Guid.NewGuid(), Key = "MAX_FIND_DRIVER_RADIUS", Value = 5, DataUnit = SettingDataUnit.KILOMETERS },
                    new Setting { Id = Guid.NewGuid(), Key = "FIND_DRIVER_TIMEOUT", Value = 10, DataUnit = SettingDataUnit.MINUTES },
                    new Setting { Id = Guid.NewGuid(), Key = "DRIVER_RESPONSE_TIMEOUT", Value = 2, DataUnit = SettingDataUnit.MINUTES },
                    new Setting { Id = Guid.NewGuid(), Key = "NEAR_DESTINATION_DISTANCE", Value = 1, DataUnit = SettingDataUnit.KILOMETERS },
                    new Setting { Id = Guid.NewGuid(), Key = "DRIVER_WAGE_PERCENT", Value = 80, DataUnit = SettingDataUnit.PERCENT }
                );
        }
    }
}
