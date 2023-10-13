using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common.Dtos
{
    public class TripDto
    {
        public Guid Id { get; set; }
        public Guid DriverId { get; set; }
        public string? EndAddress { get; set; }
        public bool Isfromcompany { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public short Occupancy { get; set; }
        public short? Status { get; set; }
        public decimal? Price { get; set; }

        public UserDto? Driver { get; set; }
    }
}
