using Domain.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common.Dtos
{
    public class LocationDto
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string? Name { get; set; }
        public string? Address { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longtitude { get; set; }
        public LocationType Type { get; set; }
        public DateTime CreateTime { get; set; }
        public DateTime UpdatedTime { get; set; }
    }

    public class LocationData
    {
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public string? Address { get; set; }
    }
}
