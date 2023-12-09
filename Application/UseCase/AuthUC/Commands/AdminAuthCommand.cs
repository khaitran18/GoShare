using Application.Common.Dtos;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCase.AuthUC.Commands
{
    public class AdminAuthCommand : IRequest<AuthResponse>
    {
        public string Username { get; set; } = null!;
        public string Password { get; set; } = null!;
    }
}
