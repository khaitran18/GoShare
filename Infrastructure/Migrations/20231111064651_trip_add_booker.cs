using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    public partial class trip_add_booker : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "booker_id",
                table: "trips",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_trips_booker_id",
                table: "trips",
                column: "booker_id");

            migrationBuilder.AddForeignKey(
                name: "fk_booker_trip",
                table: "trips",
                column: "booker_id",
                principalTable: "users",
                principalColumn: "id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_booker_trip",
                table: "trips");

            migrationBuilder.DropIndex(
                name: "IX_trips_booker_id",
                table: "trips");

            migrationBuilder.DropColumn(
                name: "booker_id",
                table: "trips");
        }
    }
}
