using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Configuration;
using Domain.DataModels;

namespace Infrastructure.Data
{
    public partial class postgresContext : DbContext
    {
        private readonly IConfiguration _configuration;

        public postgresContext(DbContextOptions<postgresContext> options, IConfiguration configuration)
            : base(options)
        {
            _configuration = configuration;
        }

        public virtual DbSet<Appfeedback> Appfeedbacks { get; set; } = null!;
        public virtual DbSet<Car> Cars { get; set; } = null!;
        public virtual DbSet<Carimage> Carimages { get; set; } = null!;
        public virtual DbSet<Chat> Chats { get; set; } = null!;
        public virtual DbSet<Rating> Ratings { get; set; } = null!;
        public virtual DbSet<Trip> Trips { get; set; } = null!;
        public virtual DbSet<User> Users { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseNpgsql(_configuration["ConnectionStrings:GoShareAzure"]);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasPostgresExtension("pg_catalog", "azure")
                .HasPostgresExtension("pg_catalog", "pgaadauth")
                .HasPostgresExtension("pg_cron");

            modelBuilder.Entity<Appfeedback>(entity =>
            {
                entity.ToTable("appfeedbacks");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Content)
                    .HasColumnType("character varying")
                    .HasColumnName("content");

                entity.Property(e => e.Time)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("time");

                entity.Property(e => e.Title)
                    .HasMaxLength(50)
                    .HasColumnName("title");

                entity.Property(e => e.UserId).HasColumnName("user_id");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Appfeedbacks)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_user");
            });

            modelBuilder.Entity<Car>(entity =>
            {
                entity.ToTable("cars");

                entity.HasIndex(e => e.LicensePlateNumber, "cars_license_plate_number_key")
                    .IsUnique();

                entity.Property(e => e.Id)
                    .ValueGeneratedNever()
                    .HasColumnName("id");

                entity.Property(e => e.Capacity).HasColumnName("capacity");

                entity.Property(e => e.LicensePlateNumber)
                    .HasMaxLength(20)
                    .HasColumnName("license_plate_number");

                entity.Property(e => e.Make)
                    .HasMaxLength(25)
                    .HasColumnName("make");

                entity.Property(e => e.Model)
                    .HasMaxLength(50)
                    .HasColumnName("model");

                entity.Property(e => e.StatusId).HasColumnName("status_id");

                entity.Property(e => e.UserId).HasColumnName("user_id");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Cars)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_user_id");
            });

            modelBuilder.Entity<Carimage>(entity =>
            {
                entity.ToTable("carimages");

                entity.Property(e => e.Id)
                    .ValueGeneratedNever()
                    .HasColumnName("id");

                entity.Property(e => e.CarId).HasColumnName("car_id");

                entity.Property(e => e.Link)
                    .HasColumnType("character varying")
                    .HasColumnName("link");

                entity.Property(e => e.TypeId).HasColumnName("type_id");

                entity.HasOne(d => d.Car)
                    .WithMany(p => p.Carimages)
                    .HasForeignKey(d => d.CarId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_car");
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
                    .HasConstraintName("fk_receiver");

                entity.HasOne(d => d.SenderNavigation)
                    .WithMany(p => p.ChatSenderNavigations)
                    .HasForeignKey(d => d.Sender)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_sender");
            });

            modelBuilder.Entity<Rating>(entity =>
            {
                entity.ToTable("ratings");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Comment)
                    .HasColumnType("character varying")
                    .HasColumnName("comment");

                entity.Property(e => e.Ratee).HasColumnName("ratee");

                entity.Property(e => e.Rater).HasColumnName("rater");

                entity.Property(e => e.Rating1).HasColumnName("rating");

                entity.Property(e => e.TripId).HasColumnName("trip_id");

                entity.HasOne(d => d.RateeNavigation)
                    .WithMany(p => p.RatingRateeNavigations)
                    .HasForeignKey(d => d.Ratee)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_ratee");

                entity.HasOne(d => d.RaterNavigation)
                    .WithMany(p => p.RatingRaterNavigations)
                    .HasForeignKey(d => d.Rater)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_rater");
            });

            modelBuilder.Entity<Trip>(entity =>
            {
                entity.ToTable("trips");

                entity.Property(e => e.Id)
                    .ValueGeneratedNever()
                    .HasColumnName("id");

                entity.Property(e => e.DriverId).HasColumnName("driver_id");

                entity.Property(e => e.EndAddress)
                    .HasColumnType("character varying")
                    .HasColumnName("end_address");

                entity.Property(e => e.EndTime)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("end_time");

                entity.Property(e => e.Isfromcompany).HasColumnName("isfromcompany");

                entity.Property(e => e.Occupancy).HasColumnName("occupancy");

                entity.Property(e => e.Price).HasColumnName("price");

                entity.Property(e => e.StartTime)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("start_time");

                entity.Property(e => e.Status).HasColumnName("status");

                entity.HasOne(d => d.Driver)
                    .WithMany(p => p.Trips)
                    .HasForeignKey(d => d.DriverId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_driver");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("users");

                entity.HasIndex(e => e.Phone, "users_phone_key")
                    .IsUnique();

                entity.Property(e => e.Id)
                    .ValueGeneratedNever()
                    .HasColumnName("id");

                entity.Property(e => e.AddressUpdateDate)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("address_update_date");

                entity.Property(e => e.CompanyAddress).HasColumnName("company_address");

                entity.Property(e => e.DisabledReason)
                    .HasColumnType("character varying")
                    .HasColumnName("disabled_reason");

                entity.Property(e => e.Email)
                    .HasColumnType("character varying")
                    .HasColumnName("email");

                entity.Property(e => e.Isactive)
                    .HasColumnName("isactive")
                    .HasDefaultValueSql("true");

                entity.Property(e => e.Isdriver)
                    .HasColumnName("isdriver")
                    .HasDefaultValueSql("false");

                entity.Property(e => e.Isverify)
                    .HasColumnName("isverify")
                    .HasDefaultValueSql("false");

                entity.Property(e => e.Name)
                    .HasMaxLength(50)
                    .HasColumnName("name");

                entity.Property(e => e.Password)
                    .HasMaxLength(100)
                    .HasColumnName("password");

                entity.Property(e => e.Phone)
                    .HasMaxLength(14)
                    .HasColumnName("phone");
            });

            modelBuilder.HasSequence("jobid_seq", "cron");

            modelBuilder.HasSequence("runid_seq", "cron");

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
