using Application.Common.Dtos;
using MediatR;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Commands
{
    public class CreateTripCommand : IRequest<TripDto>
    {
        //[JsonProperty("user_id")]
        //public Guid PassengerId { get; set; }
        public string? StartLatitude { get; set; }
        public string? StartLongitude { get; set; }
        public string? StartAddress { get; set; }
        public string? EndLatitude { get; set; }
        public string? EndLongitude { get; set; }
        public string? EndAddress { get; set; }
    }
}
