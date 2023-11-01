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
        public async Task<IActionResult> CreateTrip([FromHeader(Name = "Authorization")] string? authorization, [FromBody] CreateTripCommand command)
        {
            command.Token = authorization;
            var response = await _mediator.Send(command);
            return Ok(response);
        }

        [HttpPost("confirm-passenger")]
        public async Task<IActionResult> ConfirmPassenger([FromHeader(Name = "Authorization")] string? authorization, [FromBody] ConfirmPassengerCommand command)
        {
            command.Token = authorization;
            var response = await _mediator.Send(command);
            return Ok(response);
        }

        [HttpPost("confirm-pickup")]
        public async Task<IActionResult> ConfirmPickupPassenger([FromHeader(Name = "Authorization")] string? authorization, [FromBody] ConfirmPickupPassengerCommand command)
        {
            command.Token = authorization;
            var response = await _mediator.Send(command);
            return Ok(response);
        }

        [HttpPost("end-trip")]
        public async Task<IActionResult> EndTrip([FromHeader(Name = "Authorization")] string? authorization, [FromBody] EndTripCommand command)
        {
            command.Token = authorization;
            var response = await _mediator.Send(command);
            return Ok(response);
        }

        [HttpPost("fees")]
        public async Task<IActionResult> CalculateFeesForTrip([FromHeader(Name = "Authorization")] string? authorization, [FromBody] CalculateFeesForTripCommand command)
        {
            command.Token = authorization;
            var response = await _mediator.Send(command);
            return Ok(response);
        }

        [HttpPost("cancel")]
        public async Task<IActionResult> CancelTrip([FromHeader(Name = "Authorization")] string? authorization, [FromBody] CancelTripCommand command)
        {
            command.Token = authorization;
            var response = await _mediator.Send(command);
            return Ok(response);
        }
    }
}
