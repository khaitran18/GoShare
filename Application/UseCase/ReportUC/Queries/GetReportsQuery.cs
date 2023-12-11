using Application.Common.Dtos;
using Domain.Enumerations;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCase.ReportUC.Queries
{
    public class GetReportsQuery : IRequest<PaginatedResult<ReportDto>>
    {
        //public string? SortBy { get; set; }
        public ReportStatus? Status { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
