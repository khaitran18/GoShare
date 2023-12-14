using Application.UseCase.FeeUC.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Api_Admin.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class FeeController : ControllerBase
    {
        private readonly IMediator _mediator;

        public FeeController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetFeeAndPolicies()
        {
            GetFeeAndPoliciesQuery query = new GetFeeAndPoliciesQuery();
            var response = await _mediator.Send(query);
            return Ok(response);
        }
    }
}
