using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    public partial class setting_add_trip_cancellation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.InsertData(
                table: "settings",
                columns: new[] { "id", "data_unit", "key", "value" },
                values: new object[,]
                {
                    { new Guid("2a92c58d-9fed-4655-924a-25d08e857eb4"), (short)1, "TRIP_CANCELLATION_WINDOW", 10.0 },
                    { new Guid("6d50f035-e1f6-4052-8a29-0b114a434312"), (short)6, "TRIP_CANCELLATION_LIMIT", 20.0 }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.DeleteData(
                table: "settings",
                keyColumn: "id",
                keyValue: new Guid("2a92c58d-9fed-4655-924a-25d08e857eb4"));

            migrationBuilder.DeleteData(
                table: "settings",
                keyColumn: "id",
                keyValue: new Guid("6d50f035-e1f6-4052-8a29-0b114a434312"));

        }
    }
}
