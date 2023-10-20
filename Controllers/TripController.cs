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
        [ProducesDefaultResponseType(typeof(TripDto))]
        public async Task<IActionResult> CreateTrip([FromHeader(Name = "Authorization")] string? authorization, [FromBody] CreateTripCommand command)
        {
            command.Token = authorization;
            var response = await _mediator.Send(command);
            return Ok(response);
        }

        [HttpPost("AcceptPassenger")]
        public async Task<IActionResult> AcceptPassenger([FromHeader(Name = "Authorization")] string? authorization, [FromBody] ConfirmPassengerCommand command)
        {
            command.Token = authorization;
            var response = await _mediator.Send(command);
            return Ok(response);
        }

        [HttpPost("DenyPassenger")]
        public async Task<IActionResult> DenyPassenger([FromHeader(Name = "Authorization")] string? authorization, [FromBody] ConfirmPassengerCommand command)
        {
            command.Token = authorization;
            var response = await _mediator.Send(command);
            return Ok(response);
        }

        [HttpPost("ConfirmPickup")]
        public async Task<IActionResult> ConfirmPickupPassenger([FromHeader(Name = "Authorization")] string? authorization, [FromBody] ConfirmPickupPassengerCommand command)
        {
            command.Token = authorization;
            var response = await _mediator.Send(command);
            return Ok(response);
        }

        [HttpPost("EndTrip")]
        public async Task<IActionResult> EndTrip([FromHeader(Name = "Authorization")] string? authorization, [FromBody] EndTripCommand command)
        {
            command.Token = authorization;
            var response = await _mediator.Send(command);
            return Ok(response);
        }
    }
}
