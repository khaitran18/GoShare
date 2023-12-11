using Application.Common.Dtos;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCase.ReportUC.Queries
{
    public class GetReportQuery : IRequest<ReportDto>
    {
        public Guid Id { get; set; }
    }
}
