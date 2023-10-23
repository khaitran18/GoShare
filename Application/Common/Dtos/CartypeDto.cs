using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common.Dtos
{
    public class CartypeDto
    {
        public Guid Id { get; set; }
        public short Capacity { get; set; }
        public DateTime CreateTime { get; set; }
        public DateTime UpdateTime { get; set; }
    }

    public class CartypeFeeDto
    {
        public Guid Id { get; set; }
        public short Capacity { get; set; }
        public double TotalPrice { get; set; }
    }
}
