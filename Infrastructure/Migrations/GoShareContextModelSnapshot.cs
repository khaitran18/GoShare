﻿// <auto-generated />
using System;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Infrastructure.Migrations
{
    [DbContext(typeof(GoShareContext))]
    partial class GoShareContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.22")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("Domain.DataModels.Appfeedback", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<string>("Content")
                        .IsRequired()
                        .HasColumnType("character varying")
                        .HasColumnName("content");

                    b.Property<DateTime>("CreateTime")
                        .HasColumnType("timestamp without time zone")
                        .HasColumnName("create_time");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("character varying")
                        .HasColumnName("title");

                    b.Property<DateTime>("UpdatedTime")
                        .HasColumnType("timestamp without time zone")
                        .HasColumnName("updated_time");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid")
                        .HasColumnName("user_id");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("appfeedbacks", (string)null);
                });

            modelBuilder.Entity("Domain.DataModels.Car", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<DateTime>("CreateTime")
                        .HasColumnType("timestamp without time zone")
                        .HasColumnName("create_time");

                    b.Property<string>("LicensePlate")
                        .IsRequired()
                        .HasColumnType("character varying")
                        .HasColumnName("license_plate");

                    b.Property<string>("Make")
                        .IsRequired()
                        .HasColumnType("character varying")
                        .HasColumnName("make");

                    b.Property<string>("Model")
                        .IsRequired()
                        .HasColumnType("character varying")
                        .HasColumnName("model");

                    b.Property<short>("Status")
                        .HasColumnType("smallint")
                        .HasColumnName("status");

                    b.Property<Guid>("TypeId")
                        .HasColumnType("uuid")
                        .HasColumnName("type_id");

                    b.Property<DateTime>("UpdatedTime")
                        .HasColumnType("timestamp without time zone")
                        .HasColumnName("updated_time");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid")
                        .HasColumnName("user_id");

                    b.HasKey("Id");

                    b.HasIndex("TypeId");

                    b.HasIndex("UserId")
                        .IsUnique();

                    b.ToTable("cars", (string)null);
                });

            modelBuilder.Entity("Domain.DataModels.Cartype", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<short>("Capacity")
                        .HasColumnType("smallint")
                        .HasColumnName("capacity");

                    b.Property<DateTime>("CreateTime")
                        .HasColumnType("timestamp without time zone")
                        .HasColumnName("create_time");

                    b.Property<DateTime>("UpdateTime")
                        .HasColumnType("timestamp without time zone")
                        .HasColumnName("update_time");

                    b.HasKey("Id");

                    b.ToTable("cartypes", (string)null);
                });

            modelBuilder.Entity("Domain.DataModels.Chat", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<string>("Content")
                        .IsRequired()
                        .HasColumnType("character varying")
                        .HasColumnName("content");

                    b.Property<Guid>("Receiver")
                        .HasColumnType("uuid")
                        .HasColumnName("receiver");

                    b.Property<Guid>("Sender")
                        .HasColumnType("uuid")
                        .HasColumnName("sender");

                    b.Property<DateTime>("Time")
                        .HasColumnType("timestamp without time zone")
                        .HasColumnName("time");

                    b.HasKey("Id");

                    b.HasIndex("Receiver");

                    b.HasIndex("Sender");

                    b.ToTable("chats", (string)null);
                });

            modelBuilder.Entity("Domain.DataModels.Driverdocument", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<Guid>("CarId")
                        .HasColumnType("uuid")
                        .HasColumnName("car_id");

                    b.Property<DateTime>("CreateTime")
                        .HasColumnType("timestamp without time zone")
                        .HasColumnName("create_time");

                    b.Property<short>("Type")
                        .HasColumnType("smallint")
                        .HasColumnName("type");

                    b.Property<DateTime>("UpdateTime")
                        .HasColumnType("timestamp without time zone")
                        .HasColumnName("update_time");

                    b.Property<string>("Url")
                        .IsRequired()
                        .HasColumnType("character varying")
                        .HasColumnName("url");

                    b.HasKey("Id");

                    b.HasIndex("CarId");

                    b.ToTable("driverdocuments", (string)null);
                });

            modelBuilder.Entity("Domain.DataModels.Fee", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<double>("BaseDistance")
                        .HasColumnType("double precision")
                        .HasColumnName("base_distance");

                    b.Property<double>("BasePrice")
                        .HasColumnType("double precision")
                        .HasColumnName("base_price");

                    b.Property<Guid>("CarType")
                        .HasColumnType("uuid")
                        .HasColumnName("car_type");

                    b.Property<DateTime>("CreateTime")
                        .HasColumnType("timestamp without time zone")
                        .HasColumnName("create_time");

                    b.Property<DateTime>("UpdateTime")
                        .HasColumnType("timestamp without time zone")
                        .HasColumnName("update_time");

                    b.HasKey("Id");

                    b.HasIndex("CarType");

                    b.ToTable("fees", (string)null);
                });

            modelBuilder.Entity("Domain.DataModels.Feepolicy", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<DateTime>("CreateTime")
                        .HasColumnType("timestamp without time zone")
                        .HasColumnName("create_time");

                    b.Property<Guid>("FeeId")
                        .HasColumnType("uuid")
                        .HasColumnName("fee_id");

                    b.Property<double?>("MaxDistance")
                        .HasColumnType("double precision")
                        .HasColumnName("max_distance");

                    b.Property<double>("MinDistance")
                        .HasColumnType("double precision")
                        .HasColumnName("min_distance");

                    b.Property<double>("PricePerKm")
                        .HasColumnType("double precision")
                        .HasColumnName("price_per_km");

                    b.Property<DateTime>("UpdateTime")
                        .HasColumnType("timestamp without time zone")
                        .HasColumnName("update_time");

                    b.HasKey("Id");

                    b.HasIndex("FeeId");

                    b.ToTable("feepolicies", (string)null);
                });

            modelBuilder.Entity("Domain.DataModels.Location", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<string>("Address")
                        .HasColumnType("character varying")
                        .HasColumnName("address");

                    b.Property<DateTime>("CreateTime")
                        .HasColumnType("timestamp without time zone")
                        .HasColumnName("create_time");

                    b.Property<decimal>("Latitude")
                        .HasColumnType("numeric")
                        .HasColumnName("latitude");

                    b.Property<decimal>("Longtitude")
                        .HasColumnType("numeric")
                        .HasColumnName("longtitude");

                    b.Property<short>("Type")
                        .HasColumnType("smallint")
                        .HasColumnName("type");

                    b.Property<DateTime>("UpdatedTime")
                        .HasColumnType("timestamp without time zone")
                        .HasColumnName("updated_time");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid")
                        .HasColumnName("user_id");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("locations", (string)null);
                });

            modelBuilder.Entity("Domain.DataModels.Rating", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<string>("Comment")
                        .HasColumnType("character varying")
                        .HasColumnName("comment");

                    b.Property<DateTime>("CreateTime")
                        .HasColumnType("timestamp without time zone")
                        .HasColumnName("create_time");

                    b.Property<Guid>("Ratee")
                        .HasColumnType("uuid")
                        .HasColumnName("ratee");

                    b.Property<Guid>("Rater")
                        .HasColumnType("uuid")
                        .HasColumnName("rater");

                    b.Property<short>("Rating1")
                        .HasColumnType("smallint")
                        .HasColumnName("rating");

                    b.Property<Guid>("TripId")
                        .HasColumnType("uuid")
                        .HasColumnName("trip_id");

                    b.Property<DateTime>("UpdatedTime")
                        .HasColumnType("timestamp without time zone")
                        .HasColumnName("updated_time");

                    b.HasKey("Id");

                    b.HasIndex("Ratee");

                    b.HasIndex("Rater");

                    b.HasIndex("TripId");

                    b.ToTable("ratings", (string)null);
                });

            modelBuilder.Entity("Domain.DataModels.Setting", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<short>("DataUnit")
                        .HasColumnType("smallint")
                        .HasColumnName("data_unit");

                    b.Property<string>("Key")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)")
                        .HasColumnName("key");

                    b.Property<double>("Value")
                        .HasColumnType("double precision")
                        .HasColumnName("value");

                    b.HasKey("Id");

                    b.ToTable("settings", (string)null);

                    b.HasData(
                        new
                        {
                            Id = new Guid("5ec1198e-4a90-4304-bfbd-964d07efcc26"),
                            DataUnit = (short)5,
                            Key = "FIND_DRIVER_RADIUS",
                            Value = 1.0
                        },
                        new
                        {
                            Id = new Guid("b582f1b7-c70c-4b49-94cd-97911c6efefc"),
                            DataUnit = (short)5,
                            Key = "MAX_FIND_DRIVER_RADIUS",
                            Value = 5.0
                        },
                        new
                        {
                            Id = new Guid("60c4b25e-b8eb-477a-80c6-ce0d5ea70b3b"),
                            DataUnit = (short)1,
                            Key = "FIND_DRIVER_TIMEOUT",
                            Value = 10.0
                        },
                        new
                        {
                            Id = new Guid("694ff23a-f3bf-485d-a28e-321dc3bcc7ae"),
                            DataUnit = (short)1,
                            Key = "DRIVER_RESPONSE_TIMEOUT",
                            Value = 2.0
                        },
                        new
                        {
                            Id = new Guid("5fe54d27-3ad1-49b7-9742-0e2030fb37d3"),
                            DataUnit = (short)5,
                            Key = "NEAR_DESTINATION_DISTANCE",
                            Value = 1.0
                        },
                        new
                        {
                            Id = new Guid("30811a5b-51b3-4abe-b3b8-b39856174aa5"),
                            DataUnit = (short)0,
                            Key = "DRIVER_WAGE_PERCENT",
                            Value = 80.0
                        },
                        new
                        {
                            Id = new Guid("cd2ad062-7d62-439f-963e-aebce3fe19cf"),
                            DataUnit = (short)6,
                            Key = "TRIP_CANCELLATION_LIMIT",
                            Value = 20.0
                        },
                        new
                        {
                            Id = new Guid("10702f0c-157e-4848-af27-a8b66809e4b3"),
                            DataUnit = (short)1,
                            Key = "TRIP_CANCELLATION_WINDOW",
                            Value = 10.0
                        },
                        new
                        {
                            Id = new Guid("e86b3014-8910-4bcc-ab4f-867b74fb1059"),
                            DataUnit = (short)1,
                            Key = "CANCELLATION_BAN_DURATION",
                            Value = 15.0
                        });
                });

            modelBuilder.Entity("Domain.DataModels.Trip", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<Guid>("CartypeId")
                        .HasColumnType("uuid")
                        .HasColumnName("cartype_id");

                    b.Property<DateTime>("CreateTime")
                        .HasColumnType("timestamp without time zone")
                        .HasColumnName("create_time");

                    b.Property<double>("Distance")
                        .HasColumnType("double precision")
                        .HasColumnName("distance");

                    b.Property<Guid?>("DriverId")
                        .HasColumnType("uuid")
                        .HasColumnName("driver_id");

                    b.Property<Guid>("EndLocationId")
                        .HasColumnType("uuid")
                        .HasColumnName("end_location_id");

                    b.Property<DateTime>("EndTime")
                        .HasColumnType("timestamp without time zone")
                        .HasColumnName("end_time");

                    b.Property<Guid>("PassengerId")
                        .HasColumnType("uuid")
                        .HasColumnName("passenger_id");

                    b.Property<DateTime>("PickupTime")
                        .HasColumnType("timestamp without time zone")
                        .HasColumnName("pickup_time");

                    b.Property<double>("Price")
                        .HasColumnType("double precision")
                        .HasColumnName("price");

                    b.Property<Guid>("StartLocationId")
                        .HasColumnType("uuid")
                        .HasColumnName("start_location_id");

                    b.Property<DateTime>("StartTime")
                        .HasColumnType("timestamp without time zone")
                        .HasColumnName("start_time");

                    b.Property<short>("Status")
                        .HasColumnType("smallint")
                        .HasColumnName("status");

                    b.Property<DateTime>("UpdatedTime")
                        .HasColumnType("timestamp without time zone")
                        .HasColumnName("updated_time");

                    b.HasKey("Id");

                    b.HasIndex("CartypeId");

                    b.HasIndex("DriverId");

                    b.HasIndex("EndLocationId");

                    b.HasIndex("PassengerId");

                    b.HasIndex("StartLocationId");

                    b.ToTable("trips", (string)null);
                });

            modelBuilder.Entity("Domain.DataModels.User", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<string>("AvatarUrl")
                        .HasColumnType("character varying")
                        .HasColumnName("avatar_url");

                    b.Property<DateTime>("Birth")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("timestamp without time zone")
                        .HasColumnName("birth")
                        .HasDefaultValueSql("'-infinity'::timestamp without time zone");

                    b.Property<int>("CanceledTripCount")
                        .HasColumnType("integer")
                        .HasColumnName("canceled_trip_count");

                    b.Property<DateTime?>("CancellationBanUntil")
                        .HasColumnType("timestamp without time zone")
                        .HasColumnName("cancellation_ban_until");

                    b.Property<DateTime>("CreateTime")
                        .HasColumnType("timestamp without time zone")
                        .HasColumnName("create_time");

                    b.Property<string>("DeviceToken")
                        .HasColumnType("character varying")
                        .HasColumnName("device_token");

                    b.Property<string>("DisabledReason")
                        .HasColumnType("character varying")
                        .HasColumnName("disabled_reason");

                    b.Property<short?>("Gender")
                        .HasColumnType("smallint")
                        .HasColumnName("gender");

                    b.Property<Guid?>("GuardianId")
                        .HasColumnType("uuid")
                        .HasColumnName("guardian_id");

                    b.Property<bool>("Isdriver")
                        .HasColumnType("boolean")
                        .HasColumnName("isdriver");

                    b.Property<bool>("Isverify")
                        .HasColumnType("boolean")
                        .HasColumnName("isverify");

                    b.Property<DateTime?>("LastTripCancellationTime")
                        .HasColumnType("timestamp without time zone")
                        .HasColumnName("last_trip_cancellation_time");

                    b.Property<string>("Name")
                        .IsRequired()
                        .ValueGeneratedOnAdd()
                        .HasMaxLength(50)
                        .HasColumnType("character varying")
                        .HasColumnName("name")
                        .HasDefaultValueSql("''::character varying");

                    b.Property<string>("Otp")
                        .IsRequired()
                        .HasColumnType("character varying")
                        .HasColumnName("otp");

                    b.Property<DateTime>("OtpExpiryTime")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("timestamp without time zone")
                        .HasColumnName("otp_expiry_time")
                        .HasDefaultValueSql("'-infinity'::timestamp without time zone");

                    b.Property<string>("Passcode")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("character varying")
                        .HasColumnName("passcode")
                        .HasDefaultValueSql("''::character varying");

                    b.Property<string>("PasscodeResetToken")
                        .HasColumnType("character varying")
                        .HasColumnName("passcode_reset_token");

                    b.Property<DateTime?>("PasscodeResetTokenExpiryTime")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("timestamp without time zone")
                        .HasColumnName("passcode_reset_token_expiry_time")
                        .HasDefaultValueSql("'-infinity'::timestamp without time zone");

                    b.Property<string>("Phone")
                        .IsRequired()
                        .HasMaxLength(15)
                        .HasColumnType("character varying(15)")
                        .HasColumnName("phone");

                    b.Property<string>("RefreshToken")
                        .HasColumnType("character varying")
                        .HasColumnName("refresh_token");

                    b.Property<DateTime?>("RefreshTokenExpiryTime")
                        .HasColumnType("timestamp without time zone")
                        .HasColumnName("refresh_token_expiry_time");

                    b.Property<short>("Status")
                        .HasColumnType("smallint")
                        .HasColumnName("status");

                    b.Property<DateTime>("UpdatedTime")
                        .HasColumnType("timestamp without time zone")
                        .HasColumnName("updated_time");

                    b.HasKey("Id");

                    b.HasIndex("GuardianId");

                    b.ToTable("users", (string)null);
                });

            modelBuilder.Entity("Domain.DataModels.Wallet", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<double>("Balance")
                        .HasColumnType("double precision")
                        .HasColumnName("balance");

                    b.Property<DateTime>("CreateTime")
                        .HasColumnType("timestamp without time zone")
                        .HasColumnName("create_time");

                    b.Property<short>("Type")
                        .HasColumnType("smallint")
                        .HasColumnName("type");

                    b.Property<DateTime>("UpdatedTime")
                        .HasColumnType("timestamp without time zone")
                        .HasColumnName("updated_time");

                    b.Property<Guid?>("UserId")
                        .HasColumnType("uuid")
                        .HasColumnName("user_id");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("wallets", (string)null);
                });

            modelBuilder.Entity("Domain.DataModels.Wallettransaction", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<double>("Amount")
                        .HasColumnType("double precision")
                        .HasColumnName("amount");

                    b.Property<DateTime>("CreateTime")
                        .HasColumnType("timestamp without time zone")
                        .HasColumnName("create_time");

                    b.Property<string>("ExternalTransactionId")
                        .HasColumnType("character varying")
                        .HasColumnName("external_transaction_id");

                    b.Property<short>("PaymentMethod")
                        .HasColumnType("smallint")
                        .HasColumnName("payment_method");

                    b.Property<short>("Status")
                        .HasColumnType("smallint")
                        .HasColumnName("status");

                    b.Property<Guid?>("TripId")
                        .HasColumnType("uuid")
                        .HasColumnName("trip_id");

                    b.Property<short>("Type")
                        .HasColumnType("smallint")
                        .HasColumnName("type");

                    b.Property<DateTime>("UpdatedTime")
                        .HasColumnType("timestamp without time zone")
                        .HasColumnName("updated_time");

                    b.Property<Guid>("WalletId")
                        .HasColumnType("uuid")
                        .HasColumnName("wallet_id");

                    b.HasKey("Id");

                    b.HasIndex("TripId");

                    b.HasIndex("WalletId");

                    b.ToTable("wallettransactions", (string)null);
                });

            modelBuilder.Entity("Domain.DataModels.Appfeedback", b =>
                {
                    b.HasOne("Domain.DataModels.User", "User")
                        .WithMany("Appfeedbacks")
                        .HasForeignKey("UserId")
                        .IsRequired()
                        .HasConstraintName("fk_feedback_user");

                    b.Navigation("User");
                });

            modelBuilder.Entity("Domain.DataModels.Car", b =>
                {
                    b.HasOne("Domain.DataModels.Cartype", "Type")
                        .WithMany("Cars")
                        .HasForeignKey("TypeId")
                        .IsRequired()
                        .HasConstraintName("fk_car_type");

                    b.HasOne("Domain.DataModels.User", "User")
                        .WithOne("Car")
                        .HasForeignKey("Domain.DataModels.Car", "UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_car_user");

                    b.Navigation("Type");

                    b.Navigation("User");
                });

            modelBuilder.Entity("Domain.DataModels.Chat", b =>
                {
                    b.HasOne("Domain.DataModels.User", "ReceiverNavigation")
                        .WithMany("ChatReceiverNavigations")
                        .HasForeignKey("Receiver")
                        .IsRequired()
                        .HasConstraintName("fk_user_receiver");

                    b.HasOne("Domain.DataModels.User", "SenderNavigation")
                        .WithMany("ChatSenderNavigations")
                        .HasForeignKey("Sender")
                        .IsRequired()
                        .HasConstraintName("fk_user_sender");

                    b.Navigation("ReceiverNavigation");

                    b.Navigation("SenderNavigation");
                });

            modelBuilder.Entity("Domain.DataModels.Driverdocument", b =>
                {
                    b.HasOne("Domain.DataModels.Car", "Car")
                        .WithMany("Driverdocuments")
                        .HasForeignKey("CarId")
                        .IsRequired()
                        .HasConstraintName("fk_car_document");

                    b.Navigation("Car");
                });

            modelBuilder.Entity("Domain.DataModels.Fee", b =>
                {
                    b.HasOne("Domain.DataModels.Cartype", "CarTypeNavigation")
                        .WithMany("Fees")
                        .HasForeignKey("CarType")
                        .IsRequired()
                        .HasConstraintName("fk_type_fee");

                    b.Navigation("CarTypeNavigation");
                });

            modelBuilder.Entity("Domain.DataModels.Feepolicy", b =>
                {
                    b.HasOne("Domain.DataModels.Fee", "Fee")
                        .WithMany("Feepolicies")
                        .HasForeignKey("FeeId")
                        .IsRequired()
                        .HasConstraintName("fk_fee_policy");

                    b.Navigation("Fee");
                });

            modelBuilder.Entity("Domain.DataModels.Location", b =>
                {
                    b.HasOne("Domain.DataModels.User", "User")
                        .WithMany("Locations")
                        .HasForeignKey("UserId")
                        .IsRequired()
                        .HasConstraintName("fk_user_location");

                    b.Navigation("User");
                });

            modelBuilder.Entity("Domain.DataModels.Rating", b =>
                {
                    b.HasOne("Domain.DataModels.User", "RateeNavigation")
                        .WithMany("RatingRateeNavigations")
                        .HasForeignKey("Ratee")
                        .IsRequired()
                        .HasConstraintName("fk_ratee_user");

                    b.HasOne("Domain.DataModels.User", "RaterNavigation")
                        .WithMany("RatingRaterNavigations")
                        .HasForeignKey("Rater")
                        .IsRequired()
                        .HasConstraintName("fk_rater_user");

                    b.HasOne("Domain.DataModels.Trip", "Trip")
                        .WithMany("Ratings")
                        .HasForeignKey("TripId")
                        .IsRequired()
                        .HasConstraintName("fk_rating_trip");

                    b.Navigation("RateeNavigation");

                    b.Navigation("RaterNavigation");

                    b.Navigation("Trip");
                });

            modelBuilder.Entity("Domain.DataModels.Trip", b =>
                {
                    b.HasOne("Domain.DataModels.Cartype", "Cartype")
                        .WithMany("Trips")
                        .HasForeignKey("CartypeId")
                        .IsRequired()
                        .HasConstraintName("fk_cartype_trip");

                    b.HasOne("Domain.DataModels.User", "Driver")
                        .WithMany("TripDrivers")
                        .HasForeignKey("DriverId")
                        .HasConstraintName("fk_driver_trip");

                    b.HasOne("Domain.DataModels.Location", "EndLocation")
                        .WithMany("TripEndLocations")
                        .HasForeignKey("EndLocationId")
                        .IsRequired()
                        .HasConstraintName("fk_end_location");

                    b.HasOne("Domain.DataModels.User", "Passenger")
                        .WithMany("TripPassengers")
                        .HasForeignKey("PassengerId")
                        .IsRequired()
                        .HasConstraintName("fk_passenger_trip");

                    b.HasOne("Domain.DataModels.Location", "StartLocation")
                        .WithMany("TripStartLocations")
                        .HasForeignKey("StartLocationId")
                        .IsRequired()
                        .HasConstraintName("fk_start_location");

                    b.Navigation("Cartype");

                    b.Navigation("Driver");

                    b.Navigation("EndLocation");

                    b.Navigation("Passenger");

                    b.Navigation("StartLocation");
                });

            modelBuilder.Entity("Domain.DataModels.User", b =>
                {
                    b.HasOne("Domain.DataModels.User", "Guardian")
                        .WithMany("InverseGuardian")
                        .HasForeignKey("GuardianId")
                        .HasConstraintName("fk_user_guardian");

                    b.Navigation("Guardian");
                });

            modelBuilder.Entity("Domain.DataModels.Wallet", b =>
                {
                    b.HasOne("Domain.DataModels.User", "User")
                        .WithMany("Wallets")
                        .HasForeignKey("UserId")
                        .HasConstraintName("fk_wallet_user");

                    b.Navigation("User");
                });

            modelBuilder.Entity("Domain.DataModels.Wallettransaction", b =>
                {
                    b.HasOne("Domain.DataModels.Trip", "Trip")
                        .WithMany("Wallettransactions")
                        .HasForeignKey("TripId")
                        .HasConstraintName("fk_trip_transaction");

                    b.HasOne("Domain.DataModels.Wallet", "Wallet")
                        .WithMany("Wallettransactions")
                        .HasForeignKey("WalletId")
                        .IsRequired()
                        .HasConstraintName("fk_wallet_transaction");

                    b.Navigation("Trip");

                    b.Navigation("Wallet");
                });

            modelBuilder.Entity("Domain.DataModels.Car", b =>
                {
                    b.Navigation("Driverdocuments");
                });

            modelBuilder.Entity("Domain.DataModels.Cartype", b =>
                {
                    b.Navigation("Cars");

                    b.Navigation("Fees");

                    b.Navigation("Trips");
                });

            modelBuilder.Entity("Domain.DataModels.Fee", b =>
                {
                    b.Navigation("Feepolicies");
                });

            modelBuilder.Entity("Domain.DataModels.Location", b =>
                {
                    b.Navigation("TripEndLocations");

                    b.Navigation("TripStartLocations");
                });

            modelBuilder.Entity("Domain.DataModels.Trip", b =>
                {
                    b.Navigation("Ratings");

                    b.Navigation("Wallettransactions");
                });

            modelBuilder.Entity("Domain.DataModels.User", b =>
                {
                    b.Navigation("Appfeedbacks");

                    b.Navigation("Car");

                    b.Navigation("ChatReceiverNavigations");

                    b.Navigation("ChatSenderNavigations");

                    b.Navigation("InverseGuardian");

                    b.Navigation("Locations");

                    b.Navigation("RatingRateeNavigations");

                    b.Navigation("RatingRaterNavigations");

                    b.Navigation("TripDrivers");

                    b.Navigation("TripPassengers");

                    b.Navigation("Wallets");
                });

            modelBuilder.Entity("Domain.DataModels.Wallet", b =>
                {
                    b.Navigation("Wallettransactions");
                });
#pragma warning restore 612, 618
        }
    }
}
