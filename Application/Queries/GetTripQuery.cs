using Application.Common.Dtos;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Queries
{
    public record GetTripQuery : IRequest<TripDto?>
    {
        public string TripId { get; set; } = null!;
    }
}
