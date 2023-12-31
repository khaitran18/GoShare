﻿using Application.Common;
using Application.Common.Dtos;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCase.AppfeedbackUC.Queries
{
    public class GetAppfeedbacksQuery : IRequest<PaginatedResult<AppfeedbackDto>>
    {
        public string? SortBy { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
