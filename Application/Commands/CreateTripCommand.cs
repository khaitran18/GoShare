using Application.Common.Dtos;
using Domain.Enumerations;
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
        public string? Token { get; set; }
        public decimal StartLatitude { get; set; }
        public decimal StartLongitude { get; set; }
        public string? StartAddress { get; set; }
        public decimal EndLatitude { get; set; }
        public decimal EndLongitude { get; set; }
        public string? EndAddress { get; set; }
        public Guid CartypeId { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public string? Note { get; set; }
    }
}
