﻿using Application.Common.Dtos;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCase.DriverUC.Commands
{
    public class DriverUpdateLocationCommand : IRequest<LocationDto>
    {
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
    }
}
