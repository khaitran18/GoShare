using Application.UseCase.WalletUC.Commands;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Api_Admin.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class WalletController : ControllerBase
    {
        private readonly IMediator _mediator;

        public WalletController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPut("{userId}")]
        public async Task<IActionResult> UpdateWalletBalance([FromRoute] Guid userId, [FromBody] UpdateWalletBalanceCommand command)
        {
            command.UserId = userId;
            var response = await _mediator.Send(command);
            return Ok(response);
        }
    }
}