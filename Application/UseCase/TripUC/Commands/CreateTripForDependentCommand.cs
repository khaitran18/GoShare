﻿using Application.Common.Dtos;
using Domain.Enumerations;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCase.TripUC.Commands
{
    public class CreateTripForDependentCommand : IRequest<TripDto>
    {
        public Guid DependentId { get; set; }
        public decimal EndLatitude { get; set; }
        public decimal EndLongitude { get; set; }
        public string? EndAddress { get; set; }
        public Guid CartypeId { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public string? Note { get; set; }
    }
}
