using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Commands
{
    public class CancelTripCommand : IRequest<bool>
    {
        public string? Token { get; set; }
        public Guid TripId { get; set; }
    }
}
