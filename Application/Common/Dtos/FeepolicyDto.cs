using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common.Dtos
{
    public class FeepolicyDto
    {
        public Guid Id { get; set; }
        public double MinDistance { get; set; }
        public double? MaxDistance { get; set; }
        public double PricePerKm { get; set; }
        public DateTime CreateTime { get; set; }
        public DateTime UpdateTime { get; set; }
    }
}
