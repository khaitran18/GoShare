using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Domain.DataModels;
using Application.Configuration;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Data
{
    public partial class GoShareContext : DbContext
    {
        private readonly IConfiguration? _configuration;
        public GoShareContext()
        {
        }

        public GoShareContext(DbContextOptions<GoShareContext> options, IConfiguration? configuration)
            : base(options)
        {

            _configuration = configuration;

        }

        public virtual DbSet<Appfeedback> Appfeedbacks { get; set; } = null!;
        public virtual DbSet<Car> Cars { get; set; } = null!;
        public virtual DbSet<Cartype> Cartypes { get; set; } = null!;
        public virtual DbSet<Chat> Chats { get; set; } = null!;
        public virtual DbSet<Driverdocument> Driverdocuments { get; set; } = null!;
        public virtual DbSet<Fee> Fees { get; set; } = null!;
        public virtual DbSet<Feepolicy> Feepolicies { get; set; } = null!;
        public virtual DbSet<Location> Locations { get; set; } = null!;
        public virtual DbSet<Rating> Ratings { get; set; } = null!;
        public virtual DbSet<Trip> Trips { get; set; } = null!;
        public virtual DbSet<User> Users { get; set; } = null!;
        public virtual DbSet<Wallet> Wallets { get; set; } = null!;
        public virtual DbSet<Wallettransaction> Wallettransactions { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseNpgsql(GoShareConfiguration.ConnectionString("ConnectionStrings:GoShareAzure"));
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Appfeedback>(entity =>
            {
                entity.ToTable("appfeedbacks");

                entity.Property(e => e.Id)
                    .ValueGeneratedNever()
                    .HasColumnName("id");

                entity.Property(e => e.Content)
                    .HasColumnType("character varying")
                    .HasColumnName("content");

                entity.Property(e => e.CreateTime)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("create_time");

                entity.Property(e => e.Title)
                    .HasColumnType("character varying")
                    .HasColumnName("title");

                entity.Property(e => e.UpdatedTime)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("updated_time");

                entity.Property(e => e.UserId).HasColumnName("user_id");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Appfeedbacks)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_feedback_user");
            });

            modelBuilder.Entity<Car>(entity =>
            {
                entity.ToTable("cars");

                entity.Property(e => e.Id)
                    .ValueGeneratedNever()
                    .HasColumnName("id");

                entity.Property(e => e.CreateTime)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("create_time");

                entity.Property(e => e.LicensePlate)
                    .HasColumnType("character varying")
                    .HasColumnName("license_plate");

                entity.Property(e => e.Make)
                    .HasColumnType("character varying")
                    .HasColumnName("make");

                entity.Property(e => e.Model)
                    .HasColumnType("character varying")
                    .HasColumnName("model");

                entity.Property(e => e.Status).HasColumnName("status");

                entity.Property(e => e.TypeId).HasColumnName("type_id");

                entity.Property(e => e.UpdatedTime)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("updated_time");

                entity.Property(e => e.UserId).HasColumnName("user_id");

                entity.HasOne(d => d.Type)
                    .WithMany(p => p.Cars)
                    .HasForeignKey(d => d.TypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_car_type");

                entity.HasOne(d => d.User)
                    .WithOne(p => p.Car)
                    .HasForeignKey<Car>(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_car_user");
            });

            modelBuilder.Entity<Cartype>(entity =>
            {
                entity.ToTable("cartypes");

                entity.Property(e => e.Id)
                    .ValueGeneratedNever()
                    .HasColumnName("id");

                entity.Property(e => e.Capacity).HasColumnName("capacity");

                entity.Property(e => e.CreateTime)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("create_time");

                entity.Property(e => e.UpdateTime)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("update_time");
            });

            modelBuilder.Entity<Chat>(entity =>
            {
                entity.ToTable("chats");

                entity.Property(e => e.Id)
                    .ValueGeneratedNever()
                    .HasColumnName("id");

                entity.Property(e => e.Content)
                    .HasColumnType("character varying")
                    .HasColumnName("content");

                entity.Property(e => e.Receiver).HasColumnName("receiver");

                entity.Property(e => e.Sender).HasColumnName("sender");

                entity.Property(e => e.Time)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("time");

                entity.HasOne(d => d.ReceiverNavigation)
                    .WithMany(p => p.ChatReceiverNavigations)
                    .HasForeignKey(d => d.Receiver)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_user_receiver");

                entity.HasOne(d => d.SenderNavigation)
                    .WithMany(p => p.ChatSenderNavigations)
                    .HasForeignKey(d => d.Sender)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_user_sender");
            });

            modelBuilder.Entity<Driverdocument>(entity =>
            {
                entity.ToTable("driverdocuments");

                entity.Property(e => e.Id)
                    .ValueGeneratedNever()
                    .HasColumnName("id");

                entity.Property(e => e.CarId).HasColumnName("car_id");

                entity.Property(e => e.CreateTime)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("create_time");

                entity.Property(e => e.Type).HasColumnName("type");

                entity.Property(e => e.UpdateTime)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("update_time");

                entity.Property(e => e.Url)
                    .HasColumnType("character varying")
                    .HasColumnName("url");

                entity.HasOne(d => d.Car)
                    .WithMany(p => p.Driverdocuments)
                    .HasForeignKey(d => d.CarId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_car_document");
            });

            modelBuilder.Entity<Fee>(entity =>
            {
                entity.ToTable("fees");

                entity.Property(e => e.Id)
                    .ValueGeneratedNever()
                    .HasColumnName("id");

                entity.Property(e => e.BaseDistance).HasColumnName("base_distance");

                entity.Property(e => e.BasePrice).HasColumnName("base_price");

                entity.Property(e => e.CarType).HasColumnName("car_type");

                entity.Property(e => e.CreateTime)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("create_time");

                entity.Property(e => e.UpdateTime)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("update_time");

                entity.HasOne(d => d.CarTypeNavigation)
                    .WithMany(p => p.Fees)
                    .HasForeignKey(d => d.CarType)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_type_fee");
            });

            modelBuilder.Entity<Feepolicy>(entity =>
            {
                entity.ToTable("feepolicies");

                entity.Property(e => e.Id)
                    .ValueGeneratedNever()
                    .HasColumnName("id");

                entity.Property(e => e.CreateTime)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("create_time");

                entity.Property(e => e.FeeId).HasColumnName("fee_id");

                entity.Property(e => e.MaxDistance).HasColumnName("max_distance");

                entity.Property(e => e.MinDistance).HasColumnName("min_distance");

                entity.Property(e => e.PricePerKm).HasColumnName("price_per_km");

                entity.Property(e => e.UpdateTime)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("update_time");

                entity.HasOne(d => d.Fee)
                    .WithMany(p => p.Feepolicies)
                    .HasForeignKey(d => d.FeeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_fee_policy");
            });

            modelBuilder.Entity<Location>(entity =>
            {
                entity.ToTable("locations");

                entity.Property(e => e.Id)
                    .ValueGeneratedNever()
                    .HasColumnName("id");

                entity.Property(e => e.Address)
                    .HasColumnType("character varying")
                    .HasColumnName("address");

                entity.Property(e => e.CreateTime)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("create_time");

                entity.Property(e => e.Latitude).HasColumnName("latitude");

                entity.Property(e => e.Longtitude).HasColumnName("longtitude");

                entity.Property(e => e.Type).HasColumnName("type");

                entity.Property(e => e.UpdatedTime)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("updated_time");

                entity.Property(e => e.UserId).HasColumnName("user_id");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Locations)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_user_location");
            });

            modelBuilder.Entity<Rating>(entity =>
            {
                entity.ToTable("ratings");

                entity.Property(e => e.Id)
                    .ValueGeneratedNever()
                    .HasColumnName("id");

                entity.Property(e => e.Comment)
                    .HasColumnType("character varying")
                    .HasColumnName("comment");

                entity.Property(e => e.CreateTime)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("create_time");

                entity.Property(e => e.Ratee).HasColumnName("ratee");

                entity.Property(e => e.Rater).HasColumnName("rater");

                entity.Property(e => e.Rating1).HasColumnName("rating");

                entity.Property(e => e.TripId).HasColumnName("trip_id");

                entity.Property(e => e.UpdatedTime)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("updated_time");

                entity.HasOne(d => d.RateeNavigation)
                    .WithMany(p => p.RatingRateeNavigations)
                    .HasForeignKey(d => d.Ratee)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_ratee_user");

                entity.HasOne(d => d.RaterNavigation)
                    .WithMany(p => p.RatingRaterNavigations)
                    .HasForeignKey(d => d.Rater)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_rater_user");

                entity.HasOne(d => d.Trip)
                    .WithMany(p => p.Ratings)
                    .HasForeignKey(d => d.TripId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_rating_trip");
            });

            modelBuilder.Entity<Trip>(entity =>
            {
                entity.ToTable("trips");

                entity.Property(e => e.Id)
                    .ValueGeneratedNever()
                    .HasColumnName("id");

                entity.Property(e => e.CreateTime)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("create_time");

                entity.Property(e => e.DriverId).HasColumnName("driver_id");

                entity.Property(e => e.EndLocationId).HasColumnName("end_location_id");

                entity.Property(e => e.EndTime)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("end_time");

                entity.Property(e => e.PassengerId).HasColumnName("passenger_id");

                entity.Property(e => e.PickupTime)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("pickup_time");

                entity.Property(e => e.Distance).HasColumnName("distance");

                entity.Property(e => e.Price).HasColumnName("price");

                entity.Property(e => e.StartLocationId).HasColumnName("start_location_id");

                entity.Property(e => e.StartTime)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("start_time");

                entity.Property(e => e.Status).HasColumnName("status");

                entity.Property(e => e.UpdatedTime)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("updated_time");

                entity.HasOne(d => d.Driver)
                    .WithMany(p => p.TripDrivers)
                    .HasForeignKey(d => d.DriverId)
                    .HasConstraintName("fk_driver_trip");

                entity.HasOne(d => d.EndLocation)
                    .WithMany(p => p.TripEndLocations)
                    .HasForeignKey(d => d.EndLocationId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_end_location");

                entity.HasOne(d => d.Passenger)
                    .WithMany(p => p.TripPassengers)
                    .HasForeignKey(d => d.PassengerId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_passenger_trip");

                entity.HasOne(d => d.StartLocation)
                    .WithMany(p => p.TripStartLocations)
                    .HasForeignKey(d => d.StartLocationId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_start_location");

                entity.Property(e => e.CartypeId).HasColumnName("cartype_id");

                entity.HasOne(d => d.Cartype)
                    .WithMany(p => p.Trips)
                    .HasForeignKey(d => d.CartypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_cartype_trip");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("users");

                entity.Property(e => e.Id)
                    .ValueGeneratedNever()
                    .HasColumnName("id");

                entity.Property(e => e.Name)
                    .HasMaxLength(50)
                    .HasColumnName("name");

                entity.Property(e => e.AvatarUrl)
                    .HasColumnType("character varying")
                    .HasColumnName("avatar_url");

                entity.Property(e => e.Birth)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("birth")
                    .HasDefaultValueSql("'-infinity'::timestamp without time zone");

                entity.Property(e => e.CreateTime)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("create_time");

                entity.Property(e => e.DeviceToken)
                    .HasColumnType("character varying")
                    .HasColumnName("device_token");

                entity.Property(e => e.DisabledReason)
                    .HasColumnType("character varying")
                    .HasColumnName("disabled_reason");

                entity.Property(e => e.Gender).HasColumnName("gender");

                entity.Property(e => e.GuardianId).HasColumnName("guardian_id");

                entity.Property(e => e.Isdriver).HasColumnName("isdriver");

                entity.Property(e => e.Isverify).HasColumnName("isverify");

                entity.Property(e => e.Name)
                    .HasColumnType("character varying")
                    .HasColumnName("name")
                    .HasDefaultValueSql("''::character varying");

                entity.Property(e => e.Otp)
                    .HasColumnType("character varying")
                    .HasColumnName("otp");

                entity.Property(e => e.OtpExpiryTime)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("otp_expiry_time")
                    .HasDefaultValueSql("'-infinity'::timestamp without time zone");

                entity.Property(e => e.Passcode)
                    .HasColumnType("character varying")
                    .HasColumnName("passcode")
                    .HasDefaultValueSql("''::character varying");

                entity.Property(e => e.PasscodeResetToken)
                    .HasColumnType("character varying")
                    .HasColumnName("passcode_reset_token");

                entity.Property(e => e.PasscodeResetTokenExpiryTime)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("passcode_reset_token_expiry_time")
                    .HasDefaultValueSql("'-infinity'::timestamp without time zone");

                entity.Property(e => e.Phone)
                    .HasMaxLength(15)
                    .HasColumnName("phone");

                entity.Property(e => e.RefreshToken)
                    .HasColumnType("character varying")
                    .HasColumnName("refresh_token");

                entity.Property(e => e.RefreshTokenExpiryTime)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("refresh_token_expiry_time");

                entity.Property(e => e.Status).HasColumnName("status");

                entity.Property(e => e.UpdatedTime)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("updated_time");

                entity.HasOne(d => d.Guardian)
                    .WithMany(p => p.InverseGuardian)
                    .HasForeignKey(d => d.GuardianId)
                    .HasConstraintName("fk_user_guardian");
            });

            modelBuilder.Entity<Wallet>(entity =>
            {
                entity.ToTable("wallets");

                entity.Property(e => e.Id)
                    .ValueGeneratedNever()
                    .HasColumnName("id");

                entity.Property(e => e.Balance).HasColumnName("balance");

                entity.Property(e => e.CreateTime)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("create_time");

                entity.Property(e => e.Type).HasColumnName("type");

                entity.Property(e => e.UpdatedTime)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("updated_time");

                entity.Property(e => e.UserId).HasColumnName("user_id");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Wallets)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_wallet_user");
            });

            modelBuilder.Entity<Wallettransaction>(entity =>
            {
                entity.ToTable("wallettransactions");

                entity.Property(e => e.Id)
                    .ValueGeneratedNever()
                    .HasColumnName("id");

                entity.Property(e => e.Amount).HasColumnName("amount");

                entity.Property(e => e.CreateTime)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("create_time");

                entity.Property(e => e.ExternalTransactionId)
                    .HasColumnType("character varying")
                    .HasColumnName("external_transaction_id");

                entity.Property(e => e.PaymentMethod).HasColumnName("payment_method");

                entity.Property(e => e.Status).HasColumnName("status");

                entity.Property(e => e.TripId).HasColumnName("trip_id");

                entity.Property(e => e.Type).HasColumnName("type");

                entity.Property(e => e.UpdatedTime)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("updated_time");

                entity.Property(e => e.WalletId).HasColumnName("wallet_id");

                entity.HasOne(d => d.Trip)
                    .WithMany(p => p.Wallettransactions)
                    .HasForeignKey(d => d.TripId)
                    .HasConstraintName("fk_trip_transaction");

                entity.HasOne(d => d.Wallet)
                    .WithMany(p => p.Wallettransactions)
                    .HasForeignKey(d => d.WalletId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_wallet_transaction");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
