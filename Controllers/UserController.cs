﻿using Application.Commands;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Api_Mobile.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IMediator _mediator;

        public UserController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("UpdateFcm")]
        public async Task<IActionResult> UpdateFcmToken([FromBody] UpdateFcmTokenCommand command)
        {
            var authorization = HttpContext.Request.Headers["Authorization"].ToString();
            command.Token = authorization;
            var response = await _mediator.Send(command);
            return Ok(response);
        }
    }
}
