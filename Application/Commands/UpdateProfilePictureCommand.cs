﻿using MediatR;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Commands
{
    public class UpdateProfilePictureCommand : IRequest<string>
    {
        public string? Token { get; set; }
        public IFormFile Image { get; set; } = null!;
    }
}
