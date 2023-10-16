using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    public partial class init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "cartypes",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    capacity = table.Column<short>(type: "smallint", nullable: false),
                    create_time = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    update_time = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_cartypes", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    phone = table.Column<string>(type: "character varying(15)", maxLength: 15, nullable: false),
                    isdriver = table.Column<bool>(type: "boolean", nullable: false),
                    isverify = table.Column<bool>(type: "boolean", nullable: false),
                    disabled_reason = table.Column<string>(type: "character varying", nullable: false),
                    avatar_url = table.Column<string>(type: "character varying", nullable: false),
                    device_token = table.Column<string>(type: "character varying", nullable: false),
                    guardian_id = table.Column<Guid>(type: "uuid", nullable: true),
                    status = table.Column<short>(type: "smallint", nullable: false),
                    create_time = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    updated_time = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users", x => x.id);
                    table.ForeignKey(
                        name: "fk_user_guardian",
                        column: x => x.guardian_id,
                        principalTable: "users",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "fees",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    car_type = table.Column<Guid>(type: "uuid", nullable: false),
                    base_price = table.Column<double>(type: "double precision", nullable: false),
                    base_distance = table.Column<double>(type: "double precision", nullable: false),
                    create_time = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    update_time = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_fees", x => x.id);
                    table.ForeignKey(
                        name: "fk_type_fee",
                        column: x => x.car_type,
                        principalTable: "cartypes",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "appfeedbacks",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    title = table.Column<string>(type: "character varying", nullable: false),
                    content = table.Column<string>(type: "character varying", nullable: false),
                    create_time = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    updated_time = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_appfeedbacks", x => x.id);
                    table.ForeignKey(
                        name: "fk_feedback_user",
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
                    license_plate = table.Column<string>(type: "character varying", nullable: false),
                    make = table.Column<string>(type: "character varying", nullable: false),
                    model = table.Column<string>(type: "character varying", nullable: false),
                    type_id = table.Column<Guid>(type: "uuid", nullable: false),
                    status = table.Column<short>(type: "smallint", nullable: false),
                    create_time = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    updated_time = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_cars", x => x.id);
                    table.ForeignKey(
                        name: "fk_car_type",
                        column: x => x.type_id,
                        principalTable: "cartypes",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_wallet_user",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "locations",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    address = table.Column<string>(type: "character varying", nullable: true),
                    latitude = table.Column<decimal>(type: "numeric", nullable: false),
                    longtitude = table.Column<decimal>(type: "numeric", nullable: false),
                    type = table.Column<short>(type: "smallint", nullable: false),
                    create_time = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    updated_time = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_locations", x => x.id);
                    table.ForeignKey(
                        name: "fk_user_location",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "wallets",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    balance = table.Column<double>(type: "double precision", nullable: false),
                    type = table.Column<short>(type: "smallint", nullable: false),
                    create_time = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    updated_time = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_wallets", x => x.id);
                    table.ForeignKey(
                        name: "fk_wallet_user",
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
                        name: "fk_user_receiver",
                        column: x => x.receiver,
                        principalTable: "fees",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_user_sender",
                        column: x => x.sender,
                        principalTable: "fees",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "feepolicies",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    fee_id = table.Column<Guid>(type: "uuid", nullable: false),
                    min_distance = table.Column<double>(type: "double precision", nullable: false),
                    max_distance = table.Column<double>(type: "double precision", nullable: true),
                    price_per_km = table.Column<double>(type: "double precision", nullable: false),
                    create_time = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    update_time = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_feepolicies", x => x.id);
                    table.ForeignKey(
                        name: "fk_fee_policy",
                        column: x => x.fee_id,
                        principalTable: "fees",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "driverdocuments",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    car_id = table.Column<Guid>(type: "uuid", nullable: false),
                    type = table.Column<short>(type: "smallint", nullable: false),
                    url = table.Column<string>(type: "character varying", nullable: false),
                    create_time = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    update_time = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_driverdocuments", x => x.id);
                    table.ForeignKey(
                        name: "fk_car_document",
                        column: x => x.car_id,
                        principalTable: "cars",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "trips",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    passenger_id = table.Column<Guid>(type: "uuid", nullable: false),
                    driver_id = table.Column<Guid>(type: "uuid", nullable: true),
                    start_location_id = table.Column<Guid>(type: "uuid", nullable: false),
                    end_location_id = table.Column<Guid>(type: "uuid", nullable: false),
                    start_time = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    end_time = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    pickup_time = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    price = table.Column<double>(type: "double precision", nullable: false),
                    status = table.Column<short>(type: "smallint", nullable: false),
                    create_time = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    updated_time = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_trips", x => x.id);
                    table.ForeignKey(
                        name: "fk_driver_trip",
                        column: x => x.driver_id,
                        principalTable: "users",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_end_location",
                        column: x => x.end_location_id,
                        principalTable: "locations",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_passenger_trip",
                        column: x => x.passenger_id,
                        principalTable: "users",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_start_location",
                        column: x => x.start_location_id,
                        principalTable: "locations",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "ratings",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    rater = table.Column<Guid>(type: "uuid", nullable: false),
                    ratee = table.Column<Guid>(type: "uuid", nullable: false),
                    trip_id = table.Column<Guid>(type: "uuid", nullable: false),
                    rating = table.Column<short>(type: "smallint", nullable: false),
                    comment = table.Column<string>(type: "character varying", nullable: true),
                    create_time = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    updated_time = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ratings", x => x.id);
                    table.ForeignKey(
                        name: "fk_ratee_user",
                        column: x => x.ratee,
                        principalTable: "users",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_rater_user",
                        column: x => x.rater,
                        principalTable: "users",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_rating_trip",
                        column: x => x.trip_id,
                        principalTable: "trips",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "wallettransactions",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    wallet_id = table.Column<Guid>(type: "uuid", nullable: false),
                    trip_id = table.Column<Guid>(type: "uuid", nullable: true),
                    amount = table.Column<double>(type: "double precision", nullable: false),
                    payment_method = table.Column<short>(type: "smallint", nullable: false),
                    external_transaction_id = table.Column<string>(type: "character varying", nullable: true),
                    status = table.Column<short>(type: "smallint", nullable: false),
                    type = table.Column<short>(type: "smallint", nullable: false),
                    create_time = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    updated_time = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_wallettransactions", x => x.id);
                    table.ForeignKey(
                        name: "fk_trip_transaction",
                        column: x => x.trip_id,
                        principalTable: "trips",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_wallet_transaction",
                        column: x => x.wallet_id,
                        principalTable: "wallets",
                        principalColumn: "id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_appfeedbacks_user_id",
                table: "appfeedbacks",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_cars_type_id",
                table: "cars",
                column: "type_id");

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
                name: "IX_driverdocuments_car_id",
                table: "driverdocuments",
                column: "car_id");

            migrationBuilder.CreateIndex(
                name: "IX_feepolicies_fee_id",
                table: "feepolicies",
                column: "fee_id");

            migrationBuilder.CreateIndex(
                name: "IX_fees_car_type",
                table: "fees",
                column: "car_type");

            migrationBuilder.CreateIndex(
                name: "IX_locations_user_id",
                table: "locations",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_ratings_ratee",
                table: "ratings",
                column: "ratee");

            migrationBuilder.CreateIndex(
                name: "IX_ratings_rater",
                table: "ratings",
                column: "rater");

            migrationBuilder.CreateIndex(
                name: "IX_ratings_trip_id",
                table: "ratings",
                column: "trip_id");

            migrationBuilder.CreateIndex(
                name: "IX_trips_driver_id",
                table: "trips",
                column: "driver_id");

            migrationBuilder.CreateIndex(
                name: "IX_trips_end_location_id",
                table: "trips",
                column: "end_location_id");

            migrationBuilder.CreateIndex(
                name: "IX_trips_passenger_id",
                table: "trips",
                column: "passenger_id");

            migrationBuilder.CreateIndex(
                name: "IX_trips_start_location_id",
                table: "trips",
                column: "start_location_id");

            migrationBuilder.CreateIndex(
                name: "IX_users_guardian_id",
                table: "users",
                column: "guardian_id");

            migrationBuilder.CreateIndex(
                name: "IX_wallets_user_id",
                table: "wallets",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_wallettransactions_trip_id",
                table: "wallettransactions",
                column: "trip_id");

            migrationBuilder.CreateIndex(
                name: "IX_wallettransactions_wallet_id",
                table: "wallettransactions",
                column: "wallet_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "appfeedbacks");

            migrationBuilder.DropTable(
                name: "chats");

            migrationBuilder.DropTable(
                name: "driverdocuments");

            migrationBuilder.DropTable(
                name: "feepolicies");

            migrationBuilder.DropTable(
                name: "ratings");

            migrationBuilder.DropTable(
                name: "wallettransactions");

            migrationBuilder.DropTable(
                name: "cars");

            migrationBuilder.DropTable(
                name: "fees");

            migrationBuilder.DropTable(
                name: "trips");

            migrationBuilder.DropTable(
                name: "wallets");

            migrationBuilder.DropTable(
                name: "cartypes");

            migrationBuilder.DropTable(
                name: "locations");

            migrationBuilder.DropTable(
                name: "users");
        }
    }
}
