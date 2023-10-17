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
            var response = await _mediator.Send(command);
            return Ok(response);
        }

        [HttpPost("accept")]
        public async Task<IActionResult> AcceptPassenger([FromBody] ConfirmPassengerCommand command)
        {
            var response = await _mediator.Send(command);
            return Ok(response);
        }

        [HttpPost("deny")]
        public async Task<IActionResult> DenyPassenger([FromBody] ConfirmPassengerCommand command)
        {
            var response = await _mediator.Send(command);
            return Ok(response);
        }
    }
}
