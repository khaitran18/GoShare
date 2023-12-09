﻿using Application.Commands;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Api_Admin.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class TripController : ControllerBase
    {
        private readonly IMediator _mediator;

        public TripController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("cancel/{id}")]
        public async Task<IActionResult> CancelTrip([FromRoute] Guid id)
        {
            var command = new CancelTripCommand();
            command.TripId = id;
            var response = await _mediator.Send(command);
            return Ok(response);
        }
    }
}
