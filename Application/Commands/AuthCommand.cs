﻿using Application.Common.Dtos;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Commands
{
    public record AuthCommand : IRequest<TokenResponse>
    {
        public string Phone { get; set; } = null!;
        public string Passcode { get; set; } = null!;
    }
}
