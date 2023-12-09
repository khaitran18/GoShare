using Application.Common.Dtos;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCase.TripUC.Queries
{
    public class GetTripHistoryQuery : IRequest<List<TripDto>>
    {
        public string? SortBy { get; set; }
    }
}
