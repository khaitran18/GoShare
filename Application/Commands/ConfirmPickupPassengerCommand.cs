﻿using Application.Common.Dtos;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Commands
{
    public class ConfirmPickupPassengerCommand : IRequest<TripDto>
    {
        public string? Token { get; set; }
        public Guid TripId { get; set; }
    }
}