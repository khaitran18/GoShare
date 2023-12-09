using Application.Common.Dtos;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCase.TripUC.Queries
{
    public record GetTripQuery : IRequest<TripDto?>
    {
        public string TripId { get; set; } = null!;
    }
}
