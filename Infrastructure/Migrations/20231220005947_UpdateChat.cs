using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    public partial class UpdateChat : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "settings",
                keyColumn: "id",
                keyValue: new Guid("01db472c-71ec-43f6-93d1-34f273b3aec9"));

            migrationBuilder.DeleteData(
                table: "settings",
                keyColumn: "id",
                keyValue: new Guid("121b144e-3ec0-4359-95d9-cc68e97c784b"));

            migrationBuilder.DeleteData(
                table: "settings",
                keyColumn: "id",
                keyValue: new Guid("3dc1d2c9-dceb-458a-a14b-6f01fbeaa246"));

            migrationBuilder.DeleteData(
                table: "settings",
                keyColumn: "id",
                keyValue: new Guid("76186c24-4cdf-48ac-8479-909bf69f9035"));

            migrationBuilder.DeleteData(
                table: "settings",
                keyColumn: "id",
                keyValue: new Guid("956ae167-43f5-441e-892a-6350b4dff558"));

            migrationBuilder.DeleteData(
                table: "settings",
                keyColumn: "id",
                keyValue: new Guid("982e193c-cc1f-4441-9e9c-0d36254ba50c"));

            migrationBuilder.DeleteData(
                table: "settings",
                keyColumn: "id",
                keyValue: new Guid("a4864172-9237-4aec-8266-cace72f91d65"));

            migrationBuilder.DeleteData(
                table: "settings",
                keyColumn: "id",
                keyValue: new Guid("ba00a1f9-08ed-4a00-833d-4ee59ca4cd78"));

            migrationBuilder.DeleteData(
                table: "settings",
                keyColumn: "id",
                keyValue: new Guid("bc66cad3-a597-4e93-8fe7-119dba19a6aa"));

            migrationBuilder.DeleteData(
                table: "settings",
                keyColumn: "id",
                keyValue: new Guid("c169c1ac-c520-4209-ab00-ada6fbdee411"));

            migrationBuilder.DeleteData(
                table: "settings",
                keyColumn: "id",
                keyValue: new Guid("c9abbd8b-2e03-4233-8d86-f40a9e465ea7"));

            migrationBuilder.DeleteData(
                table: "settings",
                keyColumn: "id",
                keyValue: new Guid("d0379867-6534-4a5b-9d8c-ba648ab11d40"));

            migrationBuilder.DeleteData(
                table: "settings",
                keyColumn: "id",
                keyValue: new Guid("f58c6a7f-00ff-4f30-bde6-3402f61861f9"));

            migrationBuilder.DeleteData(
                table: "settings",
                keyColumn: "id",
                keyValue: new Guid("f73f56cf-f5e6-448d-9f02-d7cdbe020cd2"));

            migrationBuilder.AddColumn<Guid>(
                name: "trip_id",
                table: "chats",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_chats_trip_id",
                table: "chats",
                column: "trip_id");

            migrationBuilder.AddForeignKey(
                name: "fk_trip_chat",
                table: "chats",
                column: "trip_id",
                principalTable: "trips",
                principalColumn: "id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_trip_chat",
                table: "chats");

            migrationBuilder.DropIndex(
                name: "IX_chats_trip_id",
                table: "chats");

            migrationBuilder.DropColumn(
                name: "trip_id",
                table: "chats");

            migrationBuilder.InsertData(
                table: "settings",
                columns: new[] { "id", "data_unit", "description", "key", "value" },
                values: new object[,]
                {
                    { new Guid("01db472c-71ec-43f6-93d1-34f273b3aec9"), (short)5, "Bước nhảy bán kính mà hệ thống sẽ tìm kiếm tài xế xung quanh vị trí của khách hàng.", "FIND_DRIVER_RADIUS", 1.0 },
                    { new Guid("121b144e-3ec0-4359-95d9-cc68e97c784b"), (short)5, "Bán kính tối đa mà hệ thống có thể mở rộng để tìm kiếm tài xế.", "MAX_FIND_DRIVER_RADIUS", 5.0 },
                    { new Guid("3dc1d2c9-dceb-458a-a14b-6f01fbeaa246"), (short)7, "Đánh giá trung bình tối thiểu mà tài xế cần đạt được để tiếp tục lái xe.", "RATING_THRESHOLD", 1.5 },
                    { new Guid("76186c24-4cdf-48ac-8479-909bf69f9035"), (short)6, "Số lần tối đa mà khách hàng có thể hủy chuyến trong một khoảng thời gian nhất định.", "TRIP_CANCELLATION_LIMIT", 20.0 },
                    { new Guid("956ae167-43f5-441e-892a-6350b4dff558"), (short)1, "Thời gian mà khách hàng sẽ bị cấm đặt chuyến sau khi hủy quá số lần cho phép.", "CANCELLATION_BAN_DURATION", 15.0 },
                    { new Guid("982e193c-cc1f-4441-9e9c-0d36254ba50c"), (short)5, "Khoảng cách tiêu chuẩn để được coi là gần điểm đến của tài xế.", "NEAR_DESTINATION_DISTANCE", 1.0 },
                    { new Guid("a4864172-9237-4aec-8266-cace72f91d65"), (short)8, "Số dư tối thiểu mà tài xế cần duy trì trong tài khoản của mình.", "MINIMUM_BALANCE_LIMIT", -100000.0 },
                    { new Guid("ba00a1f9-08ed-4a00-833d-4ee59ca4cd78"), (short)0, "Phần trăm lương mà tài xế nhận được từ tổng giá trị chuyến đi.", "DRIVER_WAGE_PERCENT", 80.0 },
                    { new Guid("bc66cad3-a597-4e93-8fe7-119dba19a6aa"), (short)7, "Ngưỡng số dư tối thiểu mà tài xế cần duy trì để hết bị cảnh báo.", "BALANCE_THRESHOLD", 0.0 },
                    { new Guid("c169c1ac-c520-4209-ab00-ada6fbdee411"), (short)3, "Khoảng thời gian mà tài xế sẽ nhận được cảnh báo nếu đánh giá của họ dưới ngưỡng cho phép.", "WARNING_DURATION", 30.0 },
                    { new Guid("c9abbd8b-2e03-4233-8d86-f40a9e465ea7"), (short)1, "Thời gian tối đa mà hệ thống sẽ chờ phản hồi xác nhận từ tài xế cho một chuyến đi.", "DRIVER_RESPONSE_TIMEOUT", 2.0 },
                    { new Guid("d0379867-6534-4a5b-9d8c-ba648ab11d40"), (short)3, "Khoảng thời gian mà tài xế phải trả nợ nếu số dư của họ thấp hơn giới hạn cho phép.", "DEBT_REPAYMENT_PERIOD", 30.0 },
                    { new Guid("f58c6a7f-00ff-4f30-bde6-3402f61861f9"), (short)1, "Khoảng thời gian mà trong đó khách hàng sẽ bị phạt nếu hủy chuyến quá số lần tối đa.", "TRIP_CANCELLATION_WINDOW", 10.0 },
                    { new Guid("f73f56cf-f5e6-448d-9f02-d7cdbe020cd2"), (short)1, "Thời gian tối đa mà hệ thống sẽ tìm kiếm tài xế cho một chuyến đi.", "FIND_DRIVER_TIMEOUT", 10.0 }
                });
        }
    }
}
