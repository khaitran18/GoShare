using System;
using System.Collections.Generic;

namespace Domain.DataModels
{
    public partial class Car
    {
        public Car()
        {
            Carimages = new HashSet<Carimage>();
        }

        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string LicensePlateNumber { get; set; } = null!;
        public string? Make { get; set; }
        public string? Model { get; set; }
        public short? Capacity { get; set; }
        public int? StatusId { get; set; }

        public virtual User User { get; set; } = null!;
        public virtual ICollection<Carimage> Carimages { get; set; }
    }
}
