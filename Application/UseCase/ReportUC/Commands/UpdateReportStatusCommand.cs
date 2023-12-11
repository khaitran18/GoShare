using Application.Common.Dtos;
using Domain.Enumerations;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCase.ReportUC.Commands
{
    public class UpdateReportStatusCommand : IRequest<ReportDto>
    {
        public Guid Id { get; set; }
        public ReportStatus Status { get; set; }
    }
}
