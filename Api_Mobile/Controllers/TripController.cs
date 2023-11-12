using Application.Commands;
using Application.Common.Dtos;
using MediatR;
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

        [HttpPost]
        public async Task<IActionResult> CreateTrip([FromBody] CreateTripCommand command)
        {
            var response = await _mediator.Send(command);
            return Ok(response);
        }

        [HttpPost("{dependentId}")]
        public async Task<IActionResult> CreateTripForDependent([FromBody] CreateTripForDependentCommand command, [FromRoute] Guid dependentId)
        {
            command.DependentId = dependentId;
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

        [HttpPost("fees")]
        public async Task<IActionResult> CalculateFeesForTrip([FromBody] CalculateFeesForTripCommand command)
        {
            var response = await _mediator.Send(command);
            return Ok(response);
        }

        [HttpPost("cancel/{id}")]
        public async Task<IActionResult> CancelTrip([FromBody] CancelTripCommand command, [FromRoute] Guid id)
        {
            command.TripId = id;
            var response = await _mediator.Send(command);
            return Ok(response);
        }
    }
}
