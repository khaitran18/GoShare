using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Commands
{
    public class ConfirmPassengerCommand : IRequest<bool>
    {
        public string? Token { get; set; }
        public Guid TripId { get; set; }
        //public Guid? DriverId { get; set; }
        public bool Accept { get; set; }
    }
}
