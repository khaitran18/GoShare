using Application.Common.Dtos;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Commands
{
    public class RateDriverCommand : IRequest<RatingDto>
    {
        public Guid TripId { get; set; }
        public short Rating { get; set; }
        public string? Comment { get; set; }
    }
}
