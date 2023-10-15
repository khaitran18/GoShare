using System;
using System.Collections.Generic;

namespace Domain.DataModels
{
    public partial class Feepolicy
    {
        public Guid Id { get; set; }
        public Guid FeeId { get; set; }
        public double MinDistance { get; set; }
        public double? MaxDistance { get; set; }
        public double PricePerKm { get; set; }
        public DateTime CreateTime { get; set; }
        public DateTime UpdateTime { get; set; }

        public virtual Fee Fee { get; set; } = null!;
    }
}
