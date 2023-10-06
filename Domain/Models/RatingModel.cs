using System;
using System.Collections.Generic;

namespace Infrastructure.DataModels
{
    public class RatingModel
    {
        public int Id { get; set; }
        public Guid Rater { get; set; }
        public Guid Ratee { get; set; }
        public Guid TripId { get; set; }
        public short Rating1 { get; set; }
        public string? Comment { get; set; }

    }
}
