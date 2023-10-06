using System;
using System.Collections.Generic;

namespace Domain.DataModels
{
    public partial class Trip
    {
        public Guid Id { get; set; }
        public Guid DriverId { get; set; }
        public string? EndAddress { get; set; }
        public bool Isfromcompany { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public short Occupancy { get; set; }
        public short? Status { get; set; }
        public decimal? Price { get; set; }

        public virtual User Driver { get; set; } = null!;
    }
}
