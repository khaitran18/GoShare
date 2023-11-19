using Application.Commands;
using Application.Common.Dtos;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Api_Mobile.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TripController : ControllerBase
    {
        private readonly IMediator _mediator;

        public TripController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("user/create-trip")]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> CreateTrip([FromBody] CreateTripCommand command)
        {
            var response = await _mediator.Send(command);
            return Ok(response);
        }

        [HttpPost("user/create-trip-for-dependent/{dependentId}")]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> CreateTripForDependent([FromBody] CreateTripForDependentCommand command, [FromRoute] Guid dependentId)
        {
            command.DependentId = dependentId;
            var response = await _mediator.Send(command);
            return Ok(response);
        }

        [HttpPost("driver/confirm-passenger/{id}")]
        [Authorize(Roles = "Driver")]
        public async Task<IActionResult> ConfirmPassenger([FromBody] ConfirmPassengerCommand command, [FromRoute] Guid id)
        {
            command.TripId = id;
            var response = await _mediator.Send(command);
            return Ok(response);
        }

        [HttpPost("driver/confirm-pickup/{id}")]
        [Authorize(Roles = "Driver")]
        public async Task<IActionResult> ConfirmPickupPassenger([FromBody] ConfirmPickupPassengerCommand command, [FromRoute] Guid id)
        {
            command.TripId = id;
            var response = await _mediator.Send(command);
            return Ok(response);
        }

        [HttpPost("driver/end-trip/{id}")]
        [Authorize(Roles = "Driver")]
        public async Task<IActionResult> EndTrip([FromBody] EndTripCommand command, [FromRoute] Guid id)
        {
            command.TripId = id;
            var response = await _mediator.Send(command);
            return Ok(response);
        }

        [HttpPost("user/fees")]
        [Authorize]
        public async Task<IActionResult> CalculateFeesForTrip([FromBody] CalculateFeesForTripCommand command)
        {
            var response = await _mediator.Send(command);
            return Ok(response);
        }

        [HttpPost("user/cancel/{id}")]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> CancelTrip([FromBody] CancelTripCommand command, [FromRoute] Guid id)
        {
            command.TripId = id;
            var response = await _mediator.Send(command);
            return Ok(response);
        }

        [HttpPost("user/rate-driver/{id}")]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> RateDriver([FromBody] RateDriverCommand command, [FromRoute] Guid id)
        {
            command.TripId = id;
            var response = await _mediator.Send(command);
            return Ok(response);
        }
    }
}
