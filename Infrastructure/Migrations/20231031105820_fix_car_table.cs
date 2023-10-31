using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    public partial class fix_car_table : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_car_user",
                table: "cars");

            migrationBuilder.AddForeignKey(
                name: "fk_car_user",
                table: "cars",
                column: "user_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.CreateIndex(
                name: "IX_cars_user_id",
                table: "cars",
                column: "user_id",
                unique: true);

        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_car_user",
                table: "cars");

            migrationBuilder.AddForeignKey(
                name: "fk_car_user",
                table: "cars",
                column: "user_id",
                principalTable: "users",
                principalColumn: "id");

            migrationBuilder.DropIndex(
                name: "IX_cars_user_id",
                table: "cars");

        }
    }
}
