using Application.Common.Dtos;
using Application.UseCase.WallettransactionUC.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Api_Admin.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class WalletTransactionController : ControllerBase
    {
        private readonly IMediator _mediator;

        public WalletTransactionController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("system")]
        public async Task<IActionResult> GetSystemTransactions()
        {
            var query = new GetSystemTransactionQuery();
            var response = await _mediator.Send(query);
            return Ok(response);
        }
    }
}
