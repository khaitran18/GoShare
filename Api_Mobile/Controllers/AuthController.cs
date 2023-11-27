using Application.Commands;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Api_Mobile.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AuthController(IMediator mediator)
        {
            _mediator = mediator;
        }
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] AuthCommand command)
        {
            var response = await _mediator.Send(command);
            return Ok(response);
        }        
        
        [HttpPost("driver/login")]
        public async Task<IActionResult> LoginDriver([FromBody] AuthDriverCommand command)
        {
            var response = await _mediator.Send(command);
            return Ok(response);
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterCommand command)
        {
            var response = await _mediator.Send(command);
            return Ok();
        }
        [HttpPost("verify")]
        public async Task<IActionResult> Verify([FromBody] VerifyCommand command)
        {
            var response = await _mediator.Send(command);
            return Ok(response);
        }
        [HttpPost("resendotp")]
        public async Task<IActionResult> ResendOtp([FromBody] ResendOtpCommand command)
        {
            var response = await _mediator.Send(command);
            return Ok();
        }
        [HttpPost("set-passcode")]
        public async Task<IActionResult> SetPasscode([FromBody] SetPasscodeCommand command)
        {
            var response = await _mediator.Send(command);
            return Ok();
        }
        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenCommand command)
        {
            var response = await _mediator.Send(command);
            return Ok(response);
        }
        [HttpPost("revoke")]
        public async Task<IActionResult> Revoke([FromBody] RevokeCommand command)
        {
            var response = await _mediator.Send(command);
            return Ok();
        }

        [HttpPost("driver-register")]
        public async Task<IActionResult> DriverRegister([FromForm] DriverRegisterCommand command)
        {
            var response = await _mediator.Send(command);
            return Ok(response);
        }
    }
}
