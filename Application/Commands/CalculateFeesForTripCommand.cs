using Application.Common.Dtos;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Commands
{
    public class CalculateFeesForTripCommand : IRequest<List<CartypeFeeDto>>
    {
        public string? Token { get; set; }
        public string? StartLatitude { get; set; }
        public string? StartLongitude { get; set; }
        public string? EndLatitude { get; set; }
        public string? EndLongitude { get; set; }
    }
}
