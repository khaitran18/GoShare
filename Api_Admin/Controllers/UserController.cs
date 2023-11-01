using Application.Commands;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Api_Admin.Controllers
{
    [Route("api/admin/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IMediator _mediator;

        public UserController(IMediator mediator)
        {
            _mediator = mediator;
        }
        [HttpPost("verify-driver/{id}")]
        public async Task<IActionResult> VerifyDriver([FromRoute] VerifyDriverCommand command)
        {
            var response = await _mediator.Send(command);
            return Ok(response);
        }
    }
}
