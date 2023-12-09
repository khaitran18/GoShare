using Application.Common.Dtos;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCase.DriverUC.Commands
{
    public class ConfirmPassengerCommand : IRequest<TripDto>
    {
        public Guid TripId { get; set; }
        public bool Accept { get; set; }
    }
}
