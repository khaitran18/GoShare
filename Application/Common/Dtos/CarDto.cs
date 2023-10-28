using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common.Dtos
{
    public class CarDto
    {
        public string LicensePlate { get; set; } = null!;
        public string Make { get; set; } = null!;
        public string Model { get; set; } = null!;
    }
}
