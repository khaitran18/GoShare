using Application.Common.Dtos;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCase.LocationUC.Commands
{
    public class CreatePlannedDestinationCommand : IRequest<LocationDto>
    {
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public string? Address { get; set; }
        public string? Name { get; set; }
        //public Guid? UserId { get; set; }
    }
}
