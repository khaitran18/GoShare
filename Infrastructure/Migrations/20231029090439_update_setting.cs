using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    public partial class update_setting : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "settings",
                keyColumn: "id",
                keyValue: new Guid("2ed4f1d4-8f8c-4c67-8f9f-3b44a0ec4588"));

            migrationBuilder.DeleteData(
                table: "settings",
                keyColumn: "id",
                keyValue: new Guid("5031b249-ee0b-4dcc-b64f-3558b73e2a46"));

            migrationBuilder.DeleteData(
                table: "settings",
                keyColumn: "id",
                keyValue: new Guid("59eafc44-2848-4551-a1c2-6045eb9cf78b"));

            migrationBuilder.DeleteData(
                table: "settings",
                keyColumn: "id",
                keyValue: new Guid("834e9b66-b090-4c60-8924-3941ec059298"));

            migrationBuilder.DeleteData(
                table: "settings",
                keyColumn: "id",
                keyValue: new Guid("ed0c6bfa-3205-4da2-9dda-fbd3bf2eaed9"));

            migrationBuilder.InsertData(
                table: "settings",
                columns: new[] { "id", "data_unit", "key", "value" },
                values: new object[,]
                {
                    { new Guid("0e3649f5-eda9-4d1b-9cf2-5e533299e782"), (short)5, "FIND_DRIVER_RADIUS", 1.0 },
                    { new Guid("1bec16e7-5576-496b-b991-e699af5c7971"), (short)1, "FIND_DRIVER_TIMEOUT", 10.0 },
                    { new Guid("76a30c46-6893-4b4a-9d0e-79d208b79e65"), (short)1, "DRIVER_RESPONSE_TIMEOUT", 2.0 },
                    { new Guid("c4257c76-ceb5-4a84-91b6-7589a5f80529"), (short)5, "NEAR_DESTINATION_DISTANCE", 1.0 },
                    { new Guid("cf03f47b-4e6c-4c84-ac4d-7f6aea5010c9"), (short)0, "DRIVER_WAGE_PERCENT", 80.0 },
                    { new Guid("f1c16187-3a19-4cba-80b4-d65872f4a3fa"), (short)5, "MAX_FIND_DRIVER_RADIUS", 5.0 }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "settings",
                keyColumn: "id",
                keyValue: new Guid("0e3649f5-eda9-4d1b-9cf2-5e533299e782"));

            migrationBuilder.DeleteData(
                table: "settings",
                keyColumn: "id",
                keyValue: new Guid("1bec16e7-5576-496b-b991-e699af5c7971"));

            migrationBuilder.DeleteData(
                table: "settings",
                keyColumn: "id",
                keyValue: new Guid("76a30c46-6893-4b4a-9d0e-79d208b79e65"));

            migrationBuilder.DeleteData(
                table: "settings",
                keyColumn: "id",
                keyValue: new Guid("c4257c76-ceb5-4a84-91b6-7589a5f80529"));

            migrationBuilder.DeleteData(
                table: "settings",
                keyColumn: "id",
                keyValue: new Guid("cf03f47b-4e6c-4c84-ac4d-7f6aea5010c9"));

            migrationBuilder.DeleteData(
                table: "settings",
                keyColumn: "id",
                keyValue: new Guid("f1c16187-3a19-4cba-80b4-d65872f4a3fa"));

            migrationBuilder.InsertData(
                table: "settings",
                columns: new[] { "id", "data_unit", "key", "value" },
                values: new object[,]
                {
                    { new Guid("2ed4f1d4-8f8c-4c67-8f9f-3b44a0ec4588"), (short)5, "FIND_DRIVER_RADIUS", 1.0 },
                    { new Guid("5031b249-ee0b-4dcc-b64f-3558b73e2a46"), (short)1, "FIND_DRIVER_TIMEOUT", 10.0 },
                    { new Guid("59eafc44-2848-4551-a1c2-6045eb9cf78b"), (short)5, "NEAR_DESTINATION_DISTANCE", 1.0 },
                    { new Guid("834e9b66-b090-4c60-8924-3941ec059298"), (short)5, "MAX_FIND_DRIVER_RADIUS", 5.0 },
                    { new Guid("ed0c6bfa-3205-4da2-9dda-fbd3bf2eaed9"), (short)1, "DRIVER_RESPONSE_TIMEOUT", 2.0 }
                });
        }
    }
}
