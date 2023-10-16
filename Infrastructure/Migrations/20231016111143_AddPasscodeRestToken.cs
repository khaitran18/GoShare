using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    public partial class AddPasscodeRestToken : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "passcode",
                table: "users",
                type: "character varying",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "name",
                table: "users",
                type: "character varying",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying",
                oldNullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "birth",
                table: "users",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "passcode_reset_token",
                table: "users",
                type: "character varying",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "passcode_reset_token_expiry_time",
                table: "users",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "birth",
                table: "users");

            migrationBuilder.DropColumn(
                name: "passcode_reset_token",
                table: "users");

            migrationBuilder.DropColumn(
                name: "passcode_reset_token_expiry_time",
                table: "users");

            migrationBuilder.AlterColumn<string>(
                name: "passcode",
                table: "users",
                type: "character varying",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying");

            migrationBuilder.AlterColumn<string>(
                name: "name",
                table: "users",
                type: "character varying",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying");
        }
    }
}
