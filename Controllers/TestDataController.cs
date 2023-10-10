using Application.Common;
using Application.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Api_Mobile.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestDataController : ControllerBase
    {
        private readonly IMediator _mediator;

        public TestDataController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> Index([FromQuery] TestQuery query)
        {
            var response = await _mediator.Send(query);
            return Ok(response);
        }
    }
}
