using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    public partial class trip_update : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "passenger_name",
                table: "trips",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "passenger_phone_number",
                table: "trips",
                type: "character varying(15)",
                maxLength: 15,
                nullable: true);

            migrationBuilder.AddColumn<short>(
                name: "type",
                table: "trips",
                type: "smallint",
                nullable: false,
                defaultValue: (short)0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "passenger_name",
                table: "trips");

            migrationBuilder.DropColumn(
                name: "passenger_phone_number",
                table: "trips");

            migrationBuilder.DropColumn(
                name: "type",
                table: "trips");
        }
    }
}
