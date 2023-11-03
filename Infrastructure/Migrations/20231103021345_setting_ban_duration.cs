using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    public partial class setting_ban_duration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.InsertData(
                table: "settings",
                columns: new[] { "id", "data_unit", "key", "value" },
                values: new object[,]
                {
                    { new Guid("e86b3014-8910-4bcc-ab4f-867b74fb1059"), (short)1, "CANCELLATION_BAN_DURATION", 15.0 }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.DeleteData(
                table: "settings",
                keyColumn: "id",
                keyValue: new Guid("e86b3014-8910-4bcc-ab4f-867b74fb1059"));

        }
    }
}
