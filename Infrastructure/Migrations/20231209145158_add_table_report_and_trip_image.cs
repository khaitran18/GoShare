using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    public partial class add_table_report_and_trip_image : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "reports",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    trip_id = table.Column<Guid>(type: "uuid", nullable: false),
                    title = table.Column<string>(type: "character varying", nullable: false),
                    description = table.Column<string>(type: "character varying", nullable: false),
                    status = table.Column<short>(type: "smallint", nullable: false),
                    create_time = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    updated_time = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_reports", x => x.id);
                    table.ForeignKey(
                        name: "fk_trip_report",
                        column: x => x.trip_id,
                        principalTable: "trips",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "tripimages",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    trip_id = table.Column<Guid>(type: "uuid", nullable: false),
                    image_url = table.Column<string>(type: "character varying", nullable: false),
                    type = table.Column<short>(type: "smallint", nullable: false),
                    create_time = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    updated_time = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tripimages", x => x.id);
                    table.ForeignKey(
                        name: "fk_trip_tripimage",
                        column: x => x.trip_id,
                        principalTable: "trips",
                        principalColumn: "id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_reports_trip_id",
                table: "reports",
                column: "trip_id");

            migrationBuilder.CreateIndex(
                name: "IX_tripimages_trip_id",
                table: "tripimages",
                column: "trip_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "reports");

            migrationBuilder.DropTable(
                name: "tripimages");
        }
    }
}
