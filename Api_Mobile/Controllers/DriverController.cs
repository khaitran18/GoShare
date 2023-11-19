using Application.Commands;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Api_Mobile.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Driver")]
    public class DriverController : ControllerBase
    {
        private readonly IMediator _mediator;
        public DriverController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("activate")]
        public async Task<IActionResult> DriverActivate([FromBody] DriverActivateCommand command)
        {
            var response = await _mediator.Send(command);
            return Ok(response);
        }

        [HttpPost("deactivate")]
        public async Task<IActionResult> DriverDeactivate([FromBody] DriverDeactivateCommand command)
        {
            var response = await _mediator.Send(command);
            return Ok(response);
        }

        [HttpPost("confirm-passenger/{id}")]
        public async Task<IActionResult> ConfirmPassenger([FromBody] ConfirmPassengerCommand command, [FromRoute] Guid id)
        {
            command.TripId = id;
            var response = await _mediator.Send(command);
            return Ok(response);
        }

        [HttpPost("confirm-pickup/{id}")]
        public async Task<IActionResult> ConfirmPickupPassenger([FromBody] ConfirmPickupPassengerCommand command, [FromRoute] Guid id)
        {
            command.TripId = id;
            var response = await _mediator.Send(command);
            return Ok(response);
        }

        [HttpPost("end-trip/{id}")]
        public async Task<IActionResult> EndTrip([FromBody] EndTripCommand command, [FromRoute] Guid id)
        {
            command.TripId = id;
            var response = await _mediator.Send(command);
            return Ok(response);
        }
    }
}
