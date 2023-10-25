using Application.Commands;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Api_Mobile.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IMediator _mediator;

        public UserController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPut("UpdateFcm")]
        public async Task<IActionResult> UpdateFcmToken([FromHeader(Name = "Authorization")] string? authorization, [FromBody] UpdateFcmTokenCommand command)
        {
            command.Token = authorization;
            var response = await _mediator.Send(command);
            return Ok(response);
        }

        [HttpPost("avatar")]
        public async Task<IActionResult> UpdateProfilePicture([FromHeader(Name = "Authorization")] string? authorization, [FromForm] UpdateProfilePictureCommand command)
        {
            command.Token = authorization;
            var response = await _mediator.Send(command);
            return Ok(response);
        }
    }
}
