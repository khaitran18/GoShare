using Domain.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common.Dtos
{
    public class TripImageDto
    {
        public Guid Id { get; set; }
        public Guid TripId { get; set; }
        public string ImageUrl { get; set; } = null!;
        public TripImageType Type { get; set; }
        public DateTime CreateTime { get; set; }
        public DateTime UpdatedTime { get; set; }
    }
}
