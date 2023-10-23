using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    public partial class fix_car_user_relationship : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_wallet_user",
                table: "cars");

            migrationBuilder.Sql(@"
                DO
                $$
                BEGIN
                    IF EXISTS(SELECT 1 FROM pg_indexes WHERE indexname = 'IX_cars_user_id') THEN
                        DROP INDEX ""IX_cars_user_id"";
                    END IF;
                END
                $$");

            migrationBuilder.AlterColumn<short>(
                name: "status",
                table: "users",
                type: "smallint",
                nullable: false,
                defaultValue: (short)0,
                oldClrType: typeof(short),
                oldType: "smallint",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "passcode_reset_token_expiry_time",
                table: "users",
                type: "timestamp without time zone",
                nullable: true,
                defaultValueSql: "'-infinity'::timestamp without time zone",
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone");

            migrationBuilder.AlterColumn<string>(
                name: "passcode",
                table: "users",
                type: "character varying",
                nullable: true,
                defaultValueSql: "''::character varying",
                oldClrType: typeof(string),
                oldType: "character varying");

            migrationBuilder.AlterColumn<DateTime>(
                name: "otp_expiry_time",
                table: "users",
                type: "timestamp without time zone",
                nullable: false,
                defaultValueSql: "'-infinity'::timestamp without time zone",
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone");

            migrationBuilder.AlterColumn<string>(
                name: "name",
                table: "users",
                type: "character varying",
                maxLength: 50,
                nullable: false,
                defaultValueSql: "''::character varying",
                oldClrType: typeof(string),
                oldType: "character varying");

            migrationBuilder.AlterColumn<DateTime>(
                name: "birth",
                table: "users",
                type: "timestamp without time zone",
                nullable: false,
                defaultValueSql: "'-infinity'::timestamp without time zone",
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone");

            migrationBuilder.AddColumn<double>(
                name: "distance",
                table: "trips",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.CreateIndex(
                name: "IX_cars_user_id",
                table: "cars",
                column: "user_id",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "fk_car_user",
                table: "cars",
                column: "user_id",
                principalTable: "users",
                principalColumn: "id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_car_user",
                table: "cars");

            migrationBuilder.DropIndex(
                name: "IX_cars_user_id",
                table: "cars");

            migrationBuilder.DropColumn(
                name: "distance",
                table: "trips");

            migrationBuilder.AlterColumn<short>(
                name: "status",
                table: "users",
                type: "smallint",
                nullable: true,
                oldClrType: typeof(short),
                oldType: "smallint");

            migrationBuilder.AlterColumn<DateTime>(
                name: "passcode_reset_token_expiry_time",
                table: "users",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone",
                oldNullable: true,
                oldDefaultValueSql: "'-infinity'::timestamp without time zone");

            migrationBuilder.AlterColumn<string>(
                name: "passcode",
                table: "users",
                type: "character varying",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying",
                oldNullable: true,
                oldDefaultValueSql: "''::character varying");

            migrationBuilder.AlterColumn<DateTime>(
                name: "otp_expiry_time",
                table: "users",
                type: "timestamp without time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone",
                oldDefaultValueSql: "'-infinity'::timestamp without time zone");

            migrationBuilder.AlterColumn<string>(
                name: "name",
                table: "users",
                type: "character varying",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying",
                oldMaxLength: 50,
                oldDefaultValueSql: "''::character varying");

            migrationBuilder.AlterColumn<DateTime>(
                name: "birth",
                table: "users",
                type: "timestamp without time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone",
                oldDefaultValueSql: "'-infinity'::timestamp without time zone");

            migrationBuilder.CreateIndex(
                name: "IX_cars_user_id",
                table: "cars",
                column: "user_id");

            migrationBuilder.AddForeignKey(
                name: "fk_wallet_user",
                table: "cars",
                column: "user_id",
                principalTable: "users",
                principalColumn: "id");
        }
    }
}
