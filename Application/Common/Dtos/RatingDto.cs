using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common.Dtos
{
    public class RatingDto
    {
        public Guid Id { get; set; }
        public Guid Rater { get; set; }
        public Guid Ratee { get; set; }
        public Guid TripId { get; set; }
        public short Rating { get; set; }
        public string? Comment { get; set; }
        public DateTime CreateTime { get; set; }
        public DateTime UpdatedTime { get; set; }
    }
}
