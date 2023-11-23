using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    public partial class trip_canceler : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "canceled_by",
                table: "trips",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_trips_canceled_by",
                table: "trips",
                column: "canceled_by");

            migrationBuilder.AddForeignKey(
                name: "fk_canceler_trip",
                table: "trips",
                column: "canceled_by",
                principalTable: "users",
                principalColumn: "id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_canceler_trip",
                table: "trips");

            migrationBuilder.DropIndex(
                name: "IX_trips_canceled_by",
                table: "trips");

            migrationBuilder.DropColumn(
                name: "canceled_by",
                table: "trips");
        }
    }
}
