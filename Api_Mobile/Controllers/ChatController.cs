using Application.Commands;
using Application.Queries;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Api_Mobile.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ChatController(IMediator mediator)
        {
            _mediator = mediator;
        }
        [HttpPost("send")]
        public async Task<IActionResult> SendMessage([FromBody] SendMessageCommand command)
        {
            var response = await _mediator.Send(command);
            return Ok(response);
        }
        [HttpGet("get/{id}")]
        public async Task<IActionResult> GetMessages([FromRoute] GetMessagesQuery query)
        {
            var response = await _mediator.Send(query);
            return Ok(response);
        }
    }
}
