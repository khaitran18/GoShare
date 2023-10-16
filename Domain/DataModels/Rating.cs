using System;
using System.Collections.Generic;

namespace Domain.DataModels
{
    public partial class Rating
    {
        public Guid Id { get; set; }
        public Guid Rater { get; set; }
        public Guid Ratee { get; set; }
        public Guid TripId { get; set; }
        public short Rating1 { get; set; }
        public string? Comment { get; set; }
        public DateTime CreateTime { get; set; }
        public DateTime UpdatedTime { get; set; }

        public virtual User RateeNavigation { get; set; } = null!;
        public virtual User RaterNavigation { get; set; } = null!;
        public virtual Trip Trip { get; set; } = null!;
    }
}
