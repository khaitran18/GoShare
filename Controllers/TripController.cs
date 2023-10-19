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
        public async Task<IActionResult> CreateTrip([FromBody] CreateTripCommand command)
        {
            var authorization = HttpContext.Request.Headers["Authorization"].ToString();
            command.Token = authorization;
            var response = await _mediator.Send(command);
            return Ok(response);
        }

        [HttpPost("AcceptPassenger")]
        public async Task<IActionResult> AcceptPassenger([FromBody] ConfirmPassengerCommand command)
        {
            var authorization = HttpContext.Request.Headers["Authorization"].ToString();
            command.Token = authorization;
            var response = await _mediator.Send(command);
            return Ok(response);
        }

        [HttpPost("DenyPassenger")]
        public async Task<IActionResult> DenyPassenger([FromBody] ConfirmPassengerCommand command)
        {
            var authorization = HttpContext.Request.Headers["Authorization"].ToString();
            command.Token = authorization;
            var response = await _mediator.Send(command);
            return Ok(response);
        }

        [HttpPost("ConfirmPickup")]
        public async Task<IActionResult> ConfirmPickupPassenger([FromBody] ConfirmPickupPassengerCommand command)
        {
            var authorization = HttpContext.Request.Headers["Authorization"].ToString();
            command.Token = authorization;
            var response = await _mediator.Send(command);
            return Ok(response);
        }
    }
}
