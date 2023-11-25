using Application.Commands;
using Application.Common;
using Application.Common.Dtos;
using Application.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Api_Mobile.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "User,Driver")]
    public class AppfeedbackController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AppfeedbackController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> CreateFeedback([FromBody] CreateFeedbackCommand command)
        {
            var feedback = await _mediator.Send(command);
            return Ok(feedback);
        }
    }
}
