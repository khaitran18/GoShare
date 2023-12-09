using Application.UseCase.WallettransactionUC.Queries;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Api_Mobile.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WalletTransactionController : ControllerBase
    {
        private readonly IMediator _mediator;
        public WalletTransactionController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetUserTransaction()
        {
            GetUserTransactionQuery query = new GetUserTransactionQuery();
            var response = await _mediator.Send(query);
            return Ok(response);
        }
    }
}
