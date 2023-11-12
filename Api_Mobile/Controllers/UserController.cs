﻿using Application.Commands;
using Application.Common.Dtos;
using Application.Queries;
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
        public async Task<IActionResult> UpdateFcmToken([FromBody] UpdateFcmTokenCommand command)
        {
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
        public async Task<IActionResult> DriverRegister([FromForm] DriverRegisterCommand command)
        {
            var response = await _mediator.Send(command);
            return Ok(response);
        }

        [HttpGet("dependents")]
        public async Task<IActionResult> GetDependents([FromQuery] GetDependentsQuery query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }
    }
}
