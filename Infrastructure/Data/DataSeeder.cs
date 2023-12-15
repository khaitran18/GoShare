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
                    new Setting { Id = Guid.NewGuid(), Key = "FIND_DRIVER_RADIUS", Value = 1, DataUnit = SettingDataUnit.KILOMETERS, Description = "Bước nhảy bán kính mà hệ thống sẽ tìm kiếm tài xế xung quanh vị trí của khách hàng." },
                    new Setting { Id = Guid.NewGuid(), Key = "MAX_FIND_DRIVER_RADIUS", Value = 5, DataUnit = SettingDataUnit.KILOMETERS, Description = "Bán kính tối đa mà hệ thống có thể mở rộng để tìm kiếm tài xế." },
                    new Setting { Id = Guid.NewGuid(), Key = "FIND_DRIVER_TIMEOUT", Value = 10, DataUnit = SettingDataUnit.MINUTES, Description = "Thời gian tối đa mà hệ thống sẽ tìm kiếm tài xế cho một chuyến đi." },
                    new Setting { Id = Guid.NewGuid(), Key = "DRIVER_RESPONSE_TIMEOUT", Value = 2, DataUnit = SettingDataUnit.MINUTES, Description = "Thời gian tối đa mà hệ thống sẽ chờ phản hồi xác nhận từ tài xế cho một chuyến đi." },
                    new Setting { Id = Guid.NewGuid(), Key = "NEAR_DESTINATION_DISTANCE", Value = 1, DataUnit = SettingDataUnit.KILOMETERS, Description = "Khoảng cách tiêu chuẩn để được coi là gần điểm đến của tài xế." },
                    new Setting { Id = Guid.NewGuid(), Key = "DRIVER_WAGE_PERCENT", Value = 80, DataUnit = SettingDataUnit.PERCENT, Description = "Phần trăm lương mà tài xế nhận được từ tổng giá trị chuyến đi." },
                    new Setting { Id = Guid.NewGuid(), Key = "TRIP_CANCELLATION_LIMIT", Value = 20, DataUnit = SettingDataUnit.TIMES, Description = "Số lần tối đa mà khách hàng có thể hủy chuyến trong một khoảng thời gian nhất định." },
                    new Setting { Id = Guid.NewGuid(), Key = "TRIP_CANCELLATION_WINDOW", Value = 10, DataUnit = SettingDataUnit.MINUTES, Description = "Khoảng thời gian mà trong đó khách hàng sẽ bị phạt nếu hủy chuyến quá số lần tối đa." },
                    new Setting { Id = Guid.NewGuid(), Key = "CANCELLATION_BAN_DURATION", Value = 15, DataUnit = SettingDataUnit.MINUTES, Description = "Thời gian mà khách hàng sẽ bị cấm đặt chuyến sau khi hủy quá số lần cho phép." },
                    new Setting { Id = Guid.NewGuid(), Key = "RATING_THRESHOLD", Value = 1.5, DataUnit = SettingDataUnit.DEFAULT, Description = "Đánh giá trung bình tối thiểu mà tài xế cần đạt được để tiếp tục lái xe." },
                    new Setting { Id = Guid.NewGuid(), Key = "WARNING_DURATION", Value = 30, DataUnit = SettingDataUnit.DAYS, Description = "Khoảng thời gian mà tài xế sẽ nhận được cảnh báo nếu đánh giá của họ dưới ngưỡng cho phép." },
                    new Setting { Id = Guid.NewGuid(), Key = "MINIMUM_BALANCE_LIMIT", Value = -100000, DataUnit = SettingDataUnit.VND, Description = "Số dư tối thiểu mà tài xế cần duy trì trong tài khoản của mình." },
                    new Setting { Id = Guid.NewGuid(), Key = "DEBT_REPAYMENT_PERIOD", Value = 30, DataUnit = SettingDataUnit.DAYS, Description = "Khoảng thời gian mà tài xế phải trả nợ nếu số dư của họ thấp hơn giới hạn cho phép." },
                    new Setting { Id = Guid.NewGuid(), Key = "BALANCE_THRESHOLD", Value = 0, DataUnit = SettingDataUnit.DEFAULT, Description = "Ngưỡng số dư tối thiểu mà tài xế cần duy trì để hết bị cảnh báo." }
                );
        }
    }
}
