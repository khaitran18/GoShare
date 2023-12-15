using Application.UseCase.DriverUC.Commands;
using Application.UseCase.DriverUC.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Api_Admin.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class DriverController : ControllerBase
    {
        private readonly IMediator _mediator;

        public DriverController(IMediator mediator)
        {
            _mediator = mediator;
        }
        //[HttpGet]
        //public async Task<IActionResult> GetListDriver([FromQuery] GetDriverQuery query)
        //{
        //    var response = await _mediator.Send(query);
        //    return Ok(response);
        //}

        //[HttpGet("documents/{UserId}")]
        //public async Task<IActionResult> ViewDriverDocument([FromRoute] GetDriverDocumentQuery query)
        //{
        //    var response = await _mediator.Send(query);
        //    return Ok(response);
        //}

        [HttpPut("documents/{UserId}")]
        public async Task<IActionResult> UpdateDriverProfile([FromRoute] Guid UserId, [FromForm] DriverUpdateDocumentCommand command)
        {
            command.id = UserId;
            var response = await _mediator.Send(command);
            return Ok(response);
        }
    }
}
