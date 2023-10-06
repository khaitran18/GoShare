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
            try
            {
                var response = await _mediator.Send(query);
                if (!response.Error)
                {
                    return Ok(response);
                }
                else
                {
                    var ErrorResponse = new BaseResponse<Exception>
                    {
                        Exception = response.Exception,
                        Message = response.Message
                    };
                    return new ErrorHandling<Exception>(ErrorResponse);
                }
            }
            catch (Exception)
            {
                return StatusCode(500,"Please contact us for more information");
            }
        }
    }
}
