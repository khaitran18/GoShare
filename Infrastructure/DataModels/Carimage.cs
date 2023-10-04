using System;
using System.Collections.Generic;

namespace Infrastructure.DataModels
{
    public partial class Carimage
    {
        public Guid Id { get; set; }
        public Guid CarId { get; set; }
        public short TypeId { get; set; }
        public string Link { get; set; } = null!;

        public virtual Car Car { get; set; } = null!;
    }
}
