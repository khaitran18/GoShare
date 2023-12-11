using Domain.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common.Dtos
{
    public class ReportDto
    {
        public Guid Id { get; set; }
        public Guid TripId { get; set; }
        public string Title { get; set; } = null!;
        public string Description { get; set; } = null!;
        public ReportStatus Status { get; set; }
        public DateTime CreateTime { get; set; }
        public DateTime UpdatedTime { get; set; }

        public TripDto Trip { get; set; } = null!;
    }
}
