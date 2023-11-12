using Application.Commands;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Api_Mobile.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
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
    }
}
