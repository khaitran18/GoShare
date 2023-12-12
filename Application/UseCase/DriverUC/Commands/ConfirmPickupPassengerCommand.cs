using Application.Common.Dtos;
using MediatR;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCase.DriverUC.Commands
{
    public class ConfirmPickupPassengerCommand : IRequest<TripDto>
    {
        public Guid TripId { get; set; }
        public decimal DriverLatitude { get; set; }
        public decimal DriverLongitude { get; set; }
        public IFormFile? Image { get; set; }
    }
}
