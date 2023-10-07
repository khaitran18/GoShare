using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Infrastructure.Migrations
{
    public partial class init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "cron");

            migrationBuilder.CreateSequence(
                name: "jobid_seq",
                schema: "cron");

            migrationBuilder.CreateSequence(
                name: "runid_seq",
                schema: "cron");

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    phone = table.Column<string>(type: "character varying(14)", maxLength: 14, nullable: false),
                    password = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    email = table.Column<string>(type: "character varying", nullable: true),
                    company_address = table.Column<string>(type: "text", nullable: true),
                    address_update_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    isdriver = table.Column<bool>(type: "boolean", nullable: true, defaultValueSql: "false"),
                    isverify = table.Column<bool>(type: "boolean", nullable: true, defaultValueSql: "false"),
                    isactive = table.Column<bool>(type: "boolean", nullable: true, defaultValueSql: "true"),
                    disabled_reason = table.Column<string>(type: "character varying", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "appfeedbacks",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    title = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    content = table.Column<string>(type: "character varying", nullable: false),
                    time = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_appfeedbacks", x => x.id);
                    table.ForeignKey(
                        name: "fk_user",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "cars",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    license_plate_number = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    make = table.Column<string>(type: "character varying(25)", maxLength: 25, nullable: true),
                    model = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    capacity = table.Column<short>(type: "smallint", nullable: true),
                    status_id = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_cars", x => x.id);
                    table.ForeignKey(
                        name: "fk_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "chats",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    sender = table.Column<Guid>(type: "uuid", nullable: false),
                    receiver = table.Column<Guid>(type: "uuid", nullable: false),
                    content = table.Column<string>(type: "character varying", nullable: false),
                    time = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_chats", x => x.id);
                    table.ForeignKey(
                        name: "fk_receiver",
                        column: x => x.receiver,
                        principalTable: "users",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_sender",
                        column: x => x.sender,
                        principalTable: "users",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "ratings",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    rater = table.Column<Guid>(type: "uuid", nullable: false),
                    ratee = table.Column<Guid>(type: "uuid", nullable: false),
                    trip_id = table.Column<Guid>(type: "uuid", nullable: false),
                    rating = table.Column<short>(type: "smallint", nullable: false),
                    comment = table.Column<string>(type: "character varying", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ratings", x => x.id);
                    table.ForeignKey(
                        name: "fk_ratee",
                        column: x => x.ratee,
                        principalTable: "users",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_rater",
                        column: x => x.rater,
                        principalTable: "users",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "trips",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    driver_id = table.Column<Guid>(type: "uuid", nullable: false),
                    end_address = table.Column<string>(type: "character varying", nullable: true),
                    isfromcompany = table.Column<bool>(type: "boolean", nullable: false),
                    start_time = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    end_time = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    occupancy = table.Column<short>(type: "smallint", nullable: false),
                    status = table.Column<short>(type: "smallint", nullable: true),
                    price = table.Column<decimal>(type: "numeric", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_trips", x => x.id);
                    table.ForeignKey(
                        name: "fk_driver",
                        column: x => x.driver_id,
                        principalTable: "users",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "carimages",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    car_id = table.Column<Guid>(type: "uuid", nullable: false),
                    type_id = table.Column<short>(type: "smallint", nullable: false),
                    link = table.Column<string>(type: "character varying", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_carimages", x => x.id);
                    table.ForeignKey(
                        name: "fk_car",
                        column: x => x.car_id,
                        principalTable: "cars",
                        principalColumn: "id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_appfeedbacks_user_id",
                table: "appfeedbacks",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_carimages_car_id",
                table: "carimages",
                column: "car_id");

            migrationBuilder.CreateIndex(
                name: "cars_license_plate_number_key",
                table: "cars",
                column: "license_plate_number",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_cars_user_id",
                table: "cars",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_chats_receiver",
                table: "chats",
                column: "receiver");

            migrationBuilder.CreateIndex(
                name: "IX_chats_sender",
                table: "chats",
                column: "sender");

            migrationBuilder.CreateIndex(
                name: "IX_ratings_ratee",
                table: "ratings",
                column: "ratee");

            migrationBuilder.CreateIndex(
                name: "IX_ratings_rater",
                table: "ratings",
                column: "rater");

            migrationBuilder.CreateIndex(
                name: "IX_trips_driver_id",
                table: "trips",
                column: "driver_id");

            migrationBuilder.CreateIndex(
                name: "users_phone_key",
                table: "users",
                column: "phone",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "appfeedbacks");

            migrationBuilder.DropTable(
                name: "carimages");

            migrationBuilder.DropTable(
                name: "chats");

            migrationBuilder.DropTable(
                name: "ratings");

            migrationBuilder.DropTable(
                name: "trips");

            migrationBuilder.DropTable(
                name: "cars");

            migrationBuilder.DropTable(
                name: "users");

            migrationBuilder.DropSequence(
                name: "jobid_seq",
                schema: "cron");

            migrationBuilder.DropSequence(
                name: "runid_seq",
                schema: "cron");
        }
    }
}
