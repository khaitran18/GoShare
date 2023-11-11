using Domain.Enumerations;
using System;
using System.Collections.Generic;

namespace Domain.DataModels
{
    public partial class User
    {
        public User()
        {
            Appfeedbacks = new HashSet<Appfeedback>();
            InverseGuardian = new HashSet<User>();
            Locations = new HashSet<Location>();
            RatingRateeNavigations = new HashSet<Rating>();
            RatingRaterNavigations = new HashSet<Rating>();
            TripDrivers = new HashSet<Trip>();
            TripPassengers = new HashSet<Trip>();
            TripBookers = new HashSet<Trip>();
            Wallets = new HashSet<Wallet>();
            ChatReceiverNavigations = new HashSet<Chat>();
            ChatSenderNavigations = new HashSet<Chat>();
        }

        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string Phone { get; set; } = null!;
        public bool Isdriver { get; set; }
        public bool Isverify { get; set; }
        public string? DisabledReason { get; set; }
        public string? AvatarUrl { get; set; }
        public string? DeviceToken { get; set; }
        public Guid? GuardianId { get; set; }
        public UserStatus Status { get; set; }
        public DateTime CreateTime { get; set; }
        public DateTime UpdatedTime { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiryTime { get; set; }
        public short? Gender { get; set; }
        public string? Passcode { get; set; }
        public string? Otp { get; set; }
        public DateTime? OtpExpiryTime { get; set; }
        public DateTime Birth { get; set; }
        public string? PasscodeResetToken { get; set; }
        public DateTime? PasscodeResetTokenExpiryTime { get; set; }
        public int CanceledTripCount { get; set; }
        public DateTime? LastTripCancellationTime { get; set; }
        public DateTime? CancellationBanUntil { get; set; }

        public virtual User? Guardian { get; set; }
        public virtual Car? Car { get; set; }
        public virtual ICollection<Appfeedback> Appfeedbacks { get; set; }
        public virtual ICollection<User> InverseGuardian { get; set; }
        public virtual ICollection<Location> Locations { get; set; }
        public virtual ICollection<Rating> RatingRateeNavigations { get; set; }
        public virtual ICollection<Rating> RatingRaterNavigations { get; set; }
        public virtual ICollection<Trip> TripDrivers { get; set; }
        public virtual ICollection<Trip> TripPassengers { get; set; }
        public virtual ICollection<Trip> TripBookers { get; set; }
        public virtual ICollection<Wallet> Wallets { get; set; }
        public virtual ICollection<Chat> ChatReceiverNavigations { get; set; }
        public virtual ICollection<Chat> ChatSenderNavigations { get; set; }
    }
}
