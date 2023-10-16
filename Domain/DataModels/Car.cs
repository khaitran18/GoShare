using System;
using System.Collections.Generic;

namespace Domain.DataModels
{
    public partial class Car
    {
        public Car()
        {
            Driverdocuments = new HashSet<Driverdocument>();
        }

        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string LicensePlate { get; set; } = null!;
        public string Make { get; set; } = null!;
        public string Model { get; set; } = null!;
        public Guid TypeId { get; set; }
        public short Status { get; set; }
        public DateTime CreateTime { get; set; }
        public DateTime UpdatedTime { get; set; }

        public virtual Cartype Type { get; set; } = null!;
        public virtual User User { get; set; } = null!;
        public virtual ICollection<Driverdocument> Driverdocuments { get; set; }
    }
}
