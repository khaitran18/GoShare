using Application.Common.Dtos;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCase.UserUC.Commands
{
    public class CreateDependentCommand : IRequest<UserDto>
    {
        public string Name { get; set; } = null!;
        public short Gender { get; set; }
        public string Phone { get; set; } = null!;
        public DateTime Birth { get; set; }
    }
}
