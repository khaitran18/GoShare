using System;
using System.Collections.Generic;

namespace Infrastructure.DataModels
{
    public class CarModel
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string LicensePlateNumber { get; set; } = null!;
        public string? Make { get; set; }
        public string? Model { get; set; }
        public short? Capacity { get; set; }
        public int? StatusId { get; set; }
    }
}
