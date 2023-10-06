using Application.Common;
using Application.Common.Dtos;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Queries
{
    public record TestQuery: IRequest<BaseResponse<TestDto>>
    {
        public string? phone { get; set; }
    }
}
