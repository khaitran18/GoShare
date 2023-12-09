using Application.Common.Dtos;
using Domain.Enumerations;
using MediatR;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCase.AuthUC.Commands
{
    public record DriverRegisterCommand : IRequest<bool>
    {
        public string Phone { get; set; } = null!;
        public CarDto Car { get; set; } = null!;
        public short Capacity { get; set; }
        public List<PictureUploadDto> List { get; set; } = null!;
    }
}
