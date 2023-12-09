using Application.Common.Dtos;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCase.AuthUC.Commands
{
    public record AuthCommand : IRequest<AuthResponse>
    {
        public string Phone { get; set; } = null!;
        public string Passcode { get; set; } = null!;
    }
}
