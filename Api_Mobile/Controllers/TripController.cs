using Application.Common.Dtos;
using Application.UseCase.TripUC.Commands;
using Application.UseCase.TripUC.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Api_Mobile.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "User, Driver, Dependent, Admin")]
    public class TripController : ControllerBase
    {
        private readonly IMediator _mediator;

        public TripController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetCurrentTrip()
        {
            var query = new GetCurrentTripQuery();
            var response = await _mediator.Send(query);
            return Ok(response);
        }

        [HttpGet("{TripId}")]
        public async Task<IActionResult> GetTrip([FromRoute] GetTripQuery query)
        {
            var response = await _mediator.Send(query);
            return Ok(response);
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

        [HttpPost("phoneless")]
        public async Task<IActionResult> CreateTripForDependentWithoutPhone([FromBody] CreateTripForDependentWithoutPhoneCommand command)
        {
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
        public async Task<IActionResult> CancelTrip([FromRoute] Guid id)
        {
            var command = new CancelTripCommand();
            command.TripId = id;
            var response = await _mediator.Send(command);
            return Ok(response);
        }

        [HttpPost("rate-driver/{id}")]
        public async Task<IActionResult> RateDriver([FromBody] RateDriverCommand command, [FromRoute] Guid id)
        {
            command.TripId = id;
            var response = await _mediator.Send(command);
            return Ok(response);
        }

        [HttpGet("history")]
        public async Task<IActionResult> GetTripHistory([FromQuery] GetTripHistoryQuery query)
        {
            var response = await _mediator.Send(query);
            return Ok(response);
        }
    }
}
