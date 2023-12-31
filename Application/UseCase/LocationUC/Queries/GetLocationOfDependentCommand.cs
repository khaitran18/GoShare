﻿using Application.Common.Dtos;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCase.LocationUC.Queries
{
    public class GetLocationOfDependentCommand : IRequest<LocationDto>
    {
        public Guid DependentId { get; set; }
    }
}
