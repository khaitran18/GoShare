using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common.Dtos
{
    public class FeeDto
    {
        public Guid Id { get; set; }
        public Guid CarType { get; set; }
        public double BasePrice { get; set; }
        public double BaseDistance { get; set; }
        public DateTime CreateTime { get; set; }
        public DateTime UpdateTime { get; set; }
    }
}
