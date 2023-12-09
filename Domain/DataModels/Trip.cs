using Domain.Enumerations;
using System;
using System.Collections.Generic;

namespace Domain.DataModels
{
    public partial class Trip
    {
        public Trip()
        {
            Ratings = new HashSet<Rating>();
            Wallettransactions = new HashSet<Wallettransaction>();
        }

        public Guid Id { get; set; }
        public Guid PassengerId { get; set; }
        public string PassengerName { get; set; } = null!;
        public string? PassengerPhoneNumber { get; set; }
        public Guid? DriverId { get; set; }
        public Guid StartLocationId { get; set; }
        public Guid EndLocationId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public DateTime PickupTime { get; set; }
        public double Distance { get; set; }
        public double Price { get; set; }
        public Guid CartypeId { get; set; }
        public TripStatus Status { get; set; }
        public DateTime CreateTime { get; set; }
        public DateTime UpdatedTime { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public Guid BookerId { get; set; }
        public string? Note { get; set; }
        public Guid? CanceledBy { get; set; }
        public TripType Type { get; set; }

        public virtual User? Driver { get; set; }
        public virtual User Booker { get; set; } = null!;
        public virtual User? Canceler { get; set; }
        public virtual Location EndLocation { get; set; } = null!;
        public virtual User Passenger { get; set; } = null!;
        public virtual Location StartLocation { get; set; } = null!;
        public virtual Cartype Cartype { get; set; } = null!;
        public virtual ICollection<Rating> Ratings { get; set; }
        public virtual ICollection<Wallettransaction> Wallettransactions { get; set; }
    }
}
