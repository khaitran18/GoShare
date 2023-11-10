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
        public async Task<IActionResult> GetLocationOfDependent([FromHeader(Name = "Authorization")] string? authorization, [FromBody] GetLocationOfDependentCommand command, [FromRoute] Guid dependentId)
        {
            command.Token = authorization;
            command.DependentId = dependentId;
            var response = await _mediator.Send(command);
            return Ok(response);
        }
    }
}
