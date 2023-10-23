using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common.Dtos
{
    public class CarDto
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string LicensePlate { get; set; } = null!;
        public string Make { get; set; } = null!;
        public string Model { get; set; } = null!;
        public Guid TypeId { get; set; }
        public short Status { get; set; }
        public DateTime CreateTime { get; set; }
        public DateTime UpdatedTime { get; set; }

        public CartypeDto Type { get; set; } = null!;
        public UserDto User { get; set; } = null!;
    }
}
