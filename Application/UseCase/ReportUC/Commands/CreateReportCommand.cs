using Application.Common.Dtos;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCase.ReportUC.Commands
{
    public class CreateReportCommand : IRequest<ReportDto>
    {
        public Guid TripId { get; set; }
        public string Title { get; set; } = null!;
        public string Description { get; set; } = null!;
    }
}
