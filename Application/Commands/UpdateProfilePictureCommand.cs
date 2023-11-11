﻿using MediatR;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Commands
{
    public record UpdateProfilePictureCommand : IRequest<string>
    {
        public IFormFile Image { get; set; } = null!;
    }
}
