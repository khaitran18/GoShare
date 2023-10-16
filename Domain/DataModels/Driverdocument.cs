using System;
using System.Collections.Generic;

namespace Domain.DataModels
{
    public partial class Driverdocument
    {
        public Guid Id { get; set; }
        public Guid CarId { get; set; }
        public short Type { get; set; }
        public string Url { get; set; } = null!;
        public DateTime CreateTime { get; set; }
        public DateTime UpdateTime { get; set; }

        public virtual Car Car { get; set; } = null!;
    }
}
