using Application.Common.Dtos;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Commands
{
    public class AdminAuthCommand : IRequest<TokenResponse>
    {
        public string Username { get; set; } = null!;
        public string Password { get; set; } = null!;
    }
}
