using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    public partial class add_cartype_to_trip : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "cartype_id",
                table: "trips",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_trips_cartype_id",
                table: "trips",
                column: "cartype_id");

            migrationBuilder.AddForeignKey(
                name: "fk_cartype_trip",
                table: "trips",
                column: "cartype_id",
                principalTable: "cartypes",
                principalColumn: "id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_cartype_trip",
                table: "trips");

            migrationBuilder.DropIndex(
                name: "IX_trips_cartype_id",
                table: "trips");

            migrationBuilder.DropColumn(
                name: "cartype_id",
                table: "trips");
        }
    }
}
