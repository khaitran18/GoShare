using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    public partial class setting_add_data_unit : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<short>(
                name: "data_unit",
                table: "settings",
                type: "smallint",
                nullable: false,
                defaultValue: (short)0);

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

        protected override void Down(MigrationBuilder migrationBuilder)
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

            migrationBuilder.DropColumn(
                name: "data_unit",
                table: "settings");
        }
    }
}
