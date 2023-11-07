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

        [HttpPut("update-fcm")]
        public async Task<IActionResult> UpdateFcmToken([FromHeader(Name = "Authorization")] string? authorization, [FromBody] UpdateFcmTokenCommand command)
        {
            command.Token = authorization;
            var response = await _mediator.Send(command);
            return Ok(response);
        }

        [HttpPost("avatar")]
        public async Task<IActionResult> UpdateProfilePicture([FromForm] UpdateProfilePictureCommand command)
        {
            var response = await _mediator.Send(command);
            return Ok(response);
        }
        [HttpPost("driver-register")]
        public async Task<IActionResult> DriverRegister([FromHeader(Name = "Authorization")] string authorization, [FromForm] DriverRegisterCommand command)
        {
            command.Token = authorization;
            var response = await _mediator.Send(command);
            return Ok(response);
        }
    }
}
