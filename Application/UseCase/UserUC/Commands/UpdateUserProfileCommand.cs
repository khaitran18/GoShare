using Application.Common.Dtos;
using MediatR;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCase.UserUC.Commands
{
    public class UpdateUserProfileCommand : IRequest<UserDto>
    {
        public string Name { get; set; } = null!;
        public IFormFile? Image { get; set; }
        public short? Gender { get; set; }
        public DateTime Birth { get; set; }
    }
}
