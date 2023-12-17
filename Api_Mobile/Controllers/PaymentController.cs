using Application.UseCase.PaymentUC.Commands;
using MediatR;
using Microsoft.AspNetCore.Authorization;
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

        [Authorize(Roles = "User, Driver")]
        [HttpPost("topup")]
        public async Task<IActionResult> CreateTopupRequest([FromBody] CreateTopUpRequestCommand command)
        {
            var response = await _mediator.Send(command);
            return Ok(response);
        }

        [AllowAnonymous]
        [HttpGet("callback")]
        public async Task<IActionResult> PaymentCallback()
        {
            PaymentCallbackCommand command = new PaymentCallbackCommand();
            command.collection = Request.Query;
            await _mediator.Send(command);
            return Content("<html><body><h1 style = 'color: Green; font-size: 24px; font-family: Arial, sans-serif;'> Thanh toán thành công, vui lòng quay lại trang trước để cập nhật số dư </h1></body></html>", "text/html");
        }
    }
}
