using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    public partial class ModifyUserProperties : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "settings",
                keyColumn: "id",
                keyValue: new Guid("1d3f26a0-ca50-4868-a80c-a07f82f55989"));

            migrationBuilder.DeleteData(
                table: "settings",
                keyColumn: "id",
                keyValue: new Guid("2403e62d-01ef-4867-9c29-ebe7700004b4"));

            migrationBuilder.DeleteData(
                table: "settings",
                keyColumn: "id",
                keyValue: new Guid("9abf943d-f569-4c51-abd4-8a0ff7b3bd67"));

            migrationBuilder.DeleteData(
                table: "settings",
                keyColumn: "id",
                keyValue: new Guid("b4e600a0-25e9-4688-8d49-9ef263c53fe3"));

            migrationBuilder.DeleteData(
                table: "settings",
                keyColumn: "id",
                keyValue: new Guid("e6af5f6a-b9fc-4055-8614-b015a238ce45"));

            migrationBuilder.DeleteData(
                table: "settings",
                keyColumn: "id",
                keyValue: new Guid("fa148a46-179c-4400-a44e-d982b19116af"));

            migrationBuilder.AlterColumn<DateTime>(
                name: "otp_expiry_time",
                table: "users",
                type: "timestamp without time zone",
                nullable: true,
                defaultValueSql: "'-infinity'::timestamp without time zone",
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone",
                oldDefaultValueSql: "'-infinity'::timestamp without time zone");

            migrationBuilder.AlterColumn<string>(
                name: "otp",
                table: "users",
                type: "character varying",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying");

            migrationBuilder.InsertData(
                table: "settings",
                columns: new[] { "id", "data_unit", "key", "value" },
                values: new object[,]
                {
                    { new Guid("0c212449-117d-4514-be0e-f659293d46c1"), (short)0, "DRIVER_WAGE_PERCENT", 80.0 },
                    { new Guid("55722be1-af79-4a40-a988-75b601b59895"), (short)1, "FIND_DRIVER_TIMEOUT", 10.0 },
                    { new Guid("77caf1c2-151c-45c4-8331-e93b13a204e3"), (short)1, "DRIVER_RESPONSE_TIMEOUT", 2.0 },
                    { new Guid("8893705b-cfe7-42cf-9660-9c93d012f455"), (short)5, "NEAR_DESTINATION_DISTANCE", 1.0 },
                    { new Guid("9b4d2939-b67d-4708-b715-97388c60eaba"), (short)5, "FIND_DRIVER_RADIUS", 1.0 },
                    { new Guid("b1839ba2-5e70-480f-96c3-124a9afad856"), (short)5, "MAX_FIND_DRIVER_RADIUS", 5.0 }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "settings",
                keyColumn: "id",
                keyValue: new Guid("0c212449-117d-4514-be0e-f659293d46c1"));

            migrationBuilder.DeleteData(
                table: "settings",
                keyColumn: "id",
                keyValue: new Guid("55722be1-af79-4a40-a988-75b601b59895"));

            migrationBuilder.DeleteData(
                table: "settings",
                keyColumn: "id",
                keyValue: new Guid("77caf1c2-151c-45c4-8331-e93b13a204e3"));

            migrationBuilder.DeleteData(
                table: "settings",
                keyColumn: "id",
                keyValue: new Guid("8893705b-cfe7-42cf-9660-9c93d012f455"));

            migrationBuilder.DeleteData(
                table: "settings",
                keyColumn: "id",
                keyValue: new Guid("9b4d2939-b67d-4708-b715-97388c60eaba"));

            migrationBuilder.DeleteData(
                table: "settings",
                keyColumn: "id",
                keyValue: new Guid("b1839ba2-5e70-480f-96c3-124a9afad856"));

            migrationBuilder.AlterColumn<DateTime>(
                name: "otp_expiry_time",
                table: "users",
                type: "timestamp without time zone",
                nullable: false,
                defaultValueSql: "'-infinity'::timestamp without time zone",
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone",
                oldNullable: true,
                oldDefaultValueSql: "'-infinity'::timestamp without time zone");

            migrationBuilder.AlterColumn<string>(
                name: "otp",
                table: "users",
                type: "character varying",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying",
                oldNullable: true);

            migrationBuilder.InsertData(
                table: "settings",
                columns: new[] { "id", "data_unit", "key", "value" },
                values: new object[,]
                {
                    { new Guid("1d3f26a0-ca50-4868-a80c-a07f82f55989"), (short)5, "FIND_DRIVER_RADIUS", 1.0 },
                    { new Guid("2403e62d-01ef-4867-9c29-ebe7700004b4"), (short)1, "FIND_DRIVER_TIMEOUT", 10.0 },
                    { new Guid("9abf943d-f569-4c51-abd4-8a0ff7b3bd67"), (short)5, "NEAR_DESTINATION_DISTANCE", 1.0 },
                    { new Guid("b4e600a0-25e9-4688-8d49-9ef263c53fe3"), (short)5, "MAX_FIND_DRIVER_RADIUS", 5.0 },
                    { new Guid("e6af5f6a-b9fc-4055-8614-b015a238ce45"), (short)1, "DRIVER_RESPONSE_TIMEOUT", 2.0 },
                    { new Guid("fa148a46-179c-4400-a44e-d982b19116af"), (short)0, "DRIVER_WAGE_PERCENT", 80.0 }
                });
        }
    }
}
