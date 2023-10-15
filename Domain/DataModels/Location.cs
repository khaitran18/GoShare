using System;
using System.Collections.Generic;

namespace Domain.DataModels
{
    public partial class Location
    {
        public Location()
        {
            TripEndLocations = new HashSet<Trip>();
            TripStartLocations = new HashSet<Trip>();
        }

        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string? Address { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longtitude { get; set; }
        public short Type { get; set; }
        public DateTime CreateTime { get; set; }
        public DateTime UpdatedTime { get; set; }

        public virtual User User { get; set; } = null!;
        public virtual ICollection<Trip> TripEndLocations { get; set; }
        public virtual ICollection<Trip> TripStartLocations { get; set; }
    }
}
