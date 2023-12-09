using Application.UseCase.LocationUC.Commands;
using Application.UseCase.LocationUC.Queries;
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
        public async Task<IActionResult> GetLocationOfDependent([FromRoute] Guid dependentId)
        {
            var command = new GetLocationOfDependentCommand();
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

        [HttpDelete("{Id}")]
        public async Task<ActionResult<bool>> DeletePlannedDestination([FromRoute] DeletePlannedDestinationCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }
    }
}
