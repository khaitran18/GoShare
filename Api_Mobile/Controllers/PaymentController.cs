using Application.Commands;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Api_Mobile.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IMediator _mediator;
        public PaymentController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("topup")]
        public async Task<IActionResult> CreateTopupRequest([FromBody] CreateTopUpRequestCommand command)
        {
            var response = await _mediator.Send(command);
            return Ok(response);
        }

        [HttpGet("callback")]
        public async Task<IActionResult> PaymentCallback()
        {
            PaymentCallbackCommand command = new PaymentCallbackCommand();
            command.collection = Request.Query;
            await _mediator.Send(command);
            return Ok();
        }
    }
}
