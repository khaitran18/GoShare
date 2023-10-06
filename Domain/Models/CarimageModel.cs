using System;
using System.Collections.Generic;

namespace Infrastructure.DataModels
{
    public class CarimageModel
    {
        public Guid Id { get; set; }
        public Guid CarId { get; set; }
        public short TypeId { get; set; }
        public string Link { get; set; } = null!;
    }
}
