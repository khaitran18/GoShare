using Application.Commands;
using Application.Queries;
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

        [HttpGet("verify-driver/{UserId}")]
        public async Task<IActionResult> ViewDriverDocument([FromRoute] GetDriverDocumentQuery query)
        {
            var response = await _mediator.Send(query);
            return Ok(response);
        }

        [HttpPost("verify-driver/{id}")]
        public async Task<IActionResult> VerifyDriver([FromRoute] VerifyDriverCommand command)
        {
            var response = await _mediator.Send(command);
            return Ok(response);
        }
    }
}
