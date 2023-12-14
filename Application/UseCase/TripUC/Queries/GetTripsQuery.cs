using Application.Common.Dtos;
using Domain.Enumerations;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCase.TripUC.Queries
{
    public class GetTripsQuery : IRequest<PaginatedResult<TripDto>>
    {
        public string? SortBy { get; set; }
        public TripStatus? Status { get; set; }
        public PaymentMethod? PaymentMethod { get; set; }
        public TripType? Type { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
