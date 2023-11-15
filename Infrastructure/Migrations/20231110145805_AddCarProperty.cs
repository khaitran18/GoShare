using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    public partial class AddCarProperty : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "settings",
                keyColumn: "id",
                keyValue: new Guid("006d3e26-475e-4b7c-83fa-b8338b3fb84c"));

            migrationBuilder.DeleteData(
                table: "settings",
                keyColumn: "id",
                keyValue: new Guid("3e67d152-e320-4803-82b5-ca6728389395"));

            migrationBuilder.DeleteData(
                table: "settings",
                keyColumn: "id",
                keyValue: new Guid("3fb7c219-fe85-47e5-be57-aec8d39cc8fa"));

            migrationBuilder.DeleteData(
                table: "settings",
                keyColumn: "id",
                keyValue: new Guid("565298cc-6232-4392-aba8-ea054317949b"));

            migrationBuilder.DeleteData(
                table: "settings",
                keyColumn: "id",
                keyValue: new Guid("7a776f5d-92ba-48d4-8e02-ffc31f42f818"));

            migrationBuilder.DeleteData(
                table: "settings",
                keyColumn: "id",
                keyValue: new Guid("a6c800de-e375-4719-8943-8de4be9698aa"));

            migrationBuilder.DeleteData(
                table: "settings",
                keyColumn: "id",
                keyValue: new Guid("bb8ffbf9-80e6-4734-a082-9d3ae360cd47"));

            migrationBuilder.DeleteData(
                table: "settings",
                keyColumn: "id",
                keyValue: new Guid("d5866476-8d31-4a90-83aa-a656d9e19848"));

            migrationBuilder.DeleteData(
                table: "settings",
                keyColumn: "id",
                keyValue: new Guid("f8014901-4607-4031-b255-70436a99fc87"));

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

            migrationBuilder.AddColumn<DateTime>(
                name: "verifiedto",
                table: "cars",
                type: "timestamp without time zone",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "verifiedto",
                table: "cars");

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
                    { new Guid("006d3e26-475e-4b7c-83fa-b8338b3fb84c"), (short)5, "MAX_FIND_DRIVER_RADIUS", 5.0 },
                    { new Guid("3e67d152-e320-4803-82b5-ca6728389395"), (short)1, "DRIVER_RESPONSE_TIMEOUT", 2.0 },
                    { new Guid("3fb7c219-fe85-47e5-be57-aec8d39cc8fa"), (short)6, "TRIP_CANCELLATION_LIMIT", 20.0 },
                    { new Guid("565298cc-6232-4392-aba8-ea054317949b"), (short)1, "TRIP_CANCELLATION_WINDOW", 10.0 },
                    { new Guid("7a776f5d-92ba-48d4-8e02-ffc31f42f818"), (short)5, "NEAR_DESTINATION_DISTANCE", 1.0 },
                    { new Guid("a6c800de-e375-4719-8943-8de4be9698aa"), (short)1, "CANCELLATION_BAN_DURATION", 15.0 },
                    { new Guid("bb8ffbf9-80e6-4734-a082-9d3ae360cd47"), (short)0, "DRIVER_WAGE_PERCENT", 80.0 },
                    { new Guid("d5866476-8d31-4a90-83aa-a656d9e19848"), (short)5, "FIND_DRIVER_RADIUS", 1.0 },
                    { new Guid("f8014901-4607-4031-b255-70436a99fc87"), (short)1, "FIND_DRIVER_TIMEOUT", 10.0 }
                });
        }
    }
}
