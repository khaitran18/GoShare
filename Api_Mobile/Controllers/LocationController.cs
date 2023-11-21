using Application.Commands;
using Application.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Api_Mobile.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "User, Driver")]
    public class LocationController : ControllerBase
    {
        private readonly IMediator _mediator;

        public LocationController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("{dependentId}")]
        public async Task<IActionResult> GetLocationOfDependent([FromBody] GetLocationOfDependentCommand command, [FromRoute] Guid dependentId)
        {
            command.DependentId = dependentId;
            var response = await _mediator.Send(command);
            return Ok(response);
        }

        [HttpGet]
        public async Task<IActionResult> GetStoredLocation()
        {
            GetLocationQuery query = new GetLocationQuery();
            var response = await _mediator.Send(query);
            return Ok(response);
        }

        [HttpPost]
        public async Task<IActionResult> CreateStoredLocation([FromBody] CreatePlannedDestinationCommand command)
        {
            var response = await _mediator.Send(command);
            return Ok(response);
        }
    }
}
