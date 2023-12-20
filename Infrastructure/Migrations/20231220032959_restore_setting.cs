using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    public partial class restore_setting : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "settings",
                columns: new[] { "id", "data_unit", "description", "key", "value" },
                values: new object[,]
                {
                    { new Guid("1399aa1e-a23e-40e7-bde3-068e12390efc"), (short)1, "Thời gian tối đa mà hệ thống sẽ chờ phản hồi xác nhận từ tài xế cho một chuyến đi.", "DRIVER_RESPONSE_TIMEOUT", 2.0 },
                    { new Guid("1429d7f1-3abc-42cd-8c0e-d8b57beb456b"), (short)1, "Thời gian tối đa mà hệ thống sẽ tìm kiếm tài xế cho một chuyến đi.", "FIND_DRIVER_TIMEOUT", 10.0 },
                    { new Guid("1c461b6a-f837-4c5c-b686-2fb04259fdab"), (short)3, "Khoảng thời gian mà tài xế sẽ nhận được cảnh báo nếu đánh giá của họ dưới ngưỡng cho phép.", "WARNING_DURATION", 30.0 },
                    { new Guid("1f4c3802-f484-4253-aa7d-931e80a65d1c"), (short)0, "Phần trăm lương mà tài xế nhận được từ tổng giá trị chuyến đi.", "DRIVER_WAGE_PERCENT", 80.0 },
                    { new Guid("2bc8f930-ecc5-45a0-85df-187decda7e68"), (short)3, "Khoảng thời gian mà tài xế phải trả nợ nếu số dư của họ thấp hơn giới hạn cho phép.", "DEBT_REPAYMENT_PERIOD", 30.0 },
                    { new Guid("52c8625f-c0a4-4ef7-825a-d24bb6f7a1bf"), (short)6, "Số lần tối đa mà khách hàng có thể hủy chuyến trong một khoảng thời gian nhất định.", "TRIP_CANCELLATION_LIMIT", 20.0 },
                    { new Guid("61dce70b-4323-4b72-9070-42c36dd6bdec"), (short)7, "Đánh giá trung bình tối thiểu mà tài xế cần đạt được để tiếp tục lái xe.", "RATING_THRESHOLD", 1.5 },
                    { new Guid("6829e465-9950-414a-9b27-a5583e5507e7"), (short)1, "Thời gian mà khách hàng sẽ bị cấm đặt chuyến sau khi hủy quá số lần cho phép.", "CANCELLATION_BAN_DURATION", 15.0 },
                    { new Guid("682ed922-3d2c-4c14-b6e2-6642b9ae2f0c"), (short)8, "Số dư tối thiểu mà tài xế cần duy trì trong tài khoản của mình.", "MINIMUM_BALANCE_LIMIT", -100000.0 },
                    { new Guid("b02d4976-b868-4e81-a5fa-48afc1755aa0"), (short)1, "Khoảng thời gian mà trong đó khách hàng sẽ bị phạt nếu hủy chuyến quá số lần tối đa.", "TRIP_CANCELLATION_WINDOW", 10.0 },
                    { new Guid("b7570277-4953-4c09-bfd0-e8e83025cd60"), (short)5, "Bán kính tối đa mà hệ thống có thể mở rộng để tìm kiếm tài xế.", "MAX_FIND_DRIVER_RADIUS", 5.0 },
                    { new Guid("e5c4069a-a40d-44dd-b090-7aff89c84262"), (short)7, "Ngưỡng số dư tối thiểu mà tài xế cần duy trì để hết bị cảnh báo.", "BALANCE_THRESHOLD", 0.0 },
                    { new Guid("ff6f9c15-5624-48b6-bbe3-3bc613067041"), (short)5, "Khoảng cách tiêu chuẩn để được coi là gần điểm đến của tài xế.", "NEAR_DESTINATION_DISTANCE", 1.0 },
                    { new Guid("fffd271c-fb04-4761-a09f-5d359f6bf777"), (short)5, "Bước nhảy bán kính mà hệ thống sẽ tìm kiếm tài xế xung quanh vị trí của khách hàng.", "FIND_DRIVER_RADIUS", 1.0 }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "settings",
                keyColumn: "id",
                keyValue: new Guid("1399aa1e-a23e-40e7-bde3-068e12390efc"));

            migrationBuilder.DeleteData(
                table: "settings",
                keyColumn: "id",
                keyValue: new Guid("1429d7f1-3abc-42cd-8c0e-d8b57beb456b"));

            migrationBuilder.DeleteData(
                table: "settings",
                keyColumn: "id",
                keyValue: new Guid("1c461b6a-f837-4c5c-b686-2fb04259fdab"));

            migrationBuilder.DeleteData(
                table: "settings",
                keyColumn: "id",
                keyValue: new Guid("1f4c3802-f484-4253-aa7d-931e80a65d1c"));

            migrationBuilder.DeleteData(
                table: "settings",
                keyColumn: "id",
                keyValue: new Guid("2bc8f930-ecc5-45a0-85df-187decda7e68"));

            migrationBuilder.DeleteData(
                table: "settings",
                keyColumn: "id",
                keyValue: new Guid("52c8625f-c0a4-4ef7-825a-d24bb6f7a1bf"));

            migrationBuilder.DeleteData(
                table: "settings",
                keyColumn: "id",
                keyValue: new Guid("61dce70b-4323-4b72-9070-42c36dd6bdec"));

            migrationBuilder.DeleteData(
                table: "settings",
                keyColumn: "id",
                keyValue: new Guid("6829e465-9950-414a-9b27-a5583e5507e7"));

            migrationBuilder.DeleteData(
                table: "settings",
                keyColumn: "id",
                keyValue: new Guid("682ed922-3d2c-4c14-b6e2-6642b9ae2f0c"));

            migrationBuilder.DeleteData(
                table: "settings",
                keyColumn: "id",
                keyValue: new Guid("b02d4976-b868-4e81-a5fa-48afc1755aa0"));

            migrationBuilder.DeleteData(
                table: "settings",
                keyColumn: "id",
                keyValue: new Guid("b7570277-4953-4c09-bfd0-e8e83025cd60"));

            migrationBuilder.DeleteData(
                table: "settings",
                keyColumn: "id",
                keyValue: new Guid("e5c4069a-a40d-44dd-b090-7aff89c84262"));

            migrationBuilder.DeleteData(
                table: "settings",
                keyColumn: "id",
                keyValue: new Guid("ff6f9c15-5624-48b6-bbe3-3bc613067041"));

            migrationBuilder.DeleteData(
                table: "settings",
                keyColumn: "id",
                keyValue: new Guid("fffd271c-fb04-4761-a09f-5d359f6bf777"));
        }
    }
}
