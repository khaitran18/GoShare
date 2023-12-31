﻿using Application.UseCase.UserUC.Commands;
using Application.UseCase.UserUC.Queries;
using Domain.Enumerations;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Api_Mobile.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "User,Driver,Dependent")]
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

        [HttpGet("dependents")]
        public async Task<IActionResult> GetDependents([FromQuery] GetDependentsQuery query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }
        [HttpPost("dependent")]
        public async Task<IActionResult> CreateDependents([FromBody] CreateDependentCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpGet("profile")]
        public async Task<IActionResult> GetUserProfile()
        {
            var query = new GetUserQuery();
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [Authorize(Roles = "User")]
        [HttpGet("driver-register-code")]
        public async Task<IActionResult> GetDriverRegisterCode()
        {
            var query = new GetDriverRegisterCodeQuery();
            var result = await _mediator.Send(query);
            return Ok(result);
        }
        [HttpPut("profile")]
        public async Task<IActionResult> UpdateUserProfile([FromForm] UpdateUserProfileCommand command)
        {
            var response = await _mediator.Send(command);
            return Ok(response);
        }
        [Authorize(Roles = "Dependent")]
        [HttpGet("guardian-info")]
        public async Task<IActionResult> GetGuardianInformation()
        {
            var query = new GetGuardianInformationQuery();
            var result = await _mediator.Send(query);
            return Ok(result);
        }
    }
}
