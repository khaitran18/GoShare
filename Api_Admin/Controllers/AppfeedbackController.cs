using Application.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Api_Admin.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AppfeedbackController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AppfeedbackController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetAppfeedbacks([FromQuery] GetAppfeedbacksQuery query)
        {
            var response = await _mediator.Send(query);
            return Ok(response);
        }

        [HttpGet("{Id}")]
        public async Task<IActionResult> GetFeedback([FromRoute] GetFeedbackQuery query)
        {
            var feedback = await _mediator.Send(query);
            return Ok(feedback);
        }
    }
}
