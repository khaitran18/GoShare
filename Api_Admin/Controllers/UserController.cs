using Application.UseCase.DriverUC.Commands;
using Application.UseCase.DriverUC.Queries;
using Application.UseCase.UserUC.Commands;
using Application.UseCase.UserUC.Queries;
using Domain.Enumerations;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Api_Admin.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Authorize(Roles = "Admin")]
    public class UserController : ControllerBase
    {
        private readonly IMediator _mediator;

        public UserController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("driverdocuments/{UserId}")]
        public async Task<IActionResult> ViewDriverDocument([FromRoute] GetDriverDocumentQuery query)
        {
            var response = await _mediator.Send(query);
            return Ok(response);
        }

        [HttpPost("verify-driver")]
        public async Task<IActionResult> VerifyDriver([FromBody] VerifyDriverCommand command)
        {
            var response = await _mediator.Send(command);
            return Ok(response);
        }

        [HttpGet]
        public async Task<IActionResult> GetListUser([FromQuery] GetUsersQuery query)
        {
            var response = await _mediator.Send(query);
            return Ok(response);
        }
        [HttpGet("drivers")]
        public async Task<IActionResult> GetListDriver([FromQuery] GetDriverQuery query)
        {
            var response = await _mediator.Send(query);
            return Ok(response);
        }

        [HttpGet("{UserId}")]
        public async Task<IActionResult> GetUser([FromRoute] GetUserQuery query)
        {
            var response = await _mediator.Send(query);
            return Ok(response);
        }

        [HttpPut("ban/{id}")]
        public async Task<IActionResult> BanUser([FromRoute] Guid id, [FromBody] BanUserCommand command)
        {
            command.UserId = id;
            var response = await _mediator.Send(command);
            return Ok(response);
        }

        [HttpPut("unban/{id}")]
        public async Task<IActionResult> UnbanUser([FromRoute] Guid id)
        {
            var command = new UnbanUserCommand { UserId = id };
            var response = await _mediator.Send(command);
            return Ok(response);
        }
    }
}
