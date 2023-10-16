using System;
using System.Collections.Generic;

namespace Domain.DataModels
{
    public partial class Cartype
    {
        public Cartype()
        {
            Cars = new HashSet<Car>();
            Fees = new HashSet<Fee>();
        }

        public Guid Id { get; set; }
        public short Capacity { get; set; }
        public DateTime CreateTime { get; set; }
        public DateTime UpdateTime { get; set; }

        public virtual ICollection<Car> Cars { get; set; }
        public virtual ICollection<Fee> Fees { get; set; }
    }
}
