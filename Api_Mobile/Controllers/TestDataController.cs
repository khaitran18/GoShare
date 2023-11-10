using Application.Common;
using Application.Common.Dtos;
using Application.Common.Utilities;
using Application.Queries;
using Application.Services;
using Application.Services.Interfaces;
using Domain.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Api_Mobile.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestDataController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ITwilioVerification _verificationService;
        private readonly ISpeedSMSAPI _SpeedSMSAPI;

        public TestDataController(IMediator mediator, ITwilioVerification verificationService, ISpeedSMSAPI speedSMSAPI)
        {
            _mediator = mediator;
            _verificationService = verificationService;
            _SpeedSMSAPI = speedSMSAPI;
        }

        [HttpGet]
        public async Task<IActionResult> Index([FromQuery] TestQuery query)
        {
            var response = await _mediator.Send(query);
            return Ok(response);
        }

        [HttpPost("Twilio")]
        public async Task<IActionResult> TwilioSendOtp([FromQuery]string phone="+84919651361",string channel="sms")
        {
            var verification = await _verificationService.StartVerificationAsync(phone, channel);
            return Ok();
        }

        //[HttpPost("SpeedSMS")]
        //public async Task<IActionResult> SpeedApiSendOtp([FromQuery] string phone)
        //{
        //    var verification = await _SpeedSMSAPI.sendSMS(phone,"Ma OTP cua ban la: "+OtpUtils.Generate(),5);
        //    return Ok(verification);
        //}

        [HttpPost("TwilioVerify")]
        public async Task<IActionResult> CheckOtp([FromQuery] string phone = "+84919651361", string? code="")
        {
            var verification = await _verificationService.CheckVerificationAsync(phone,code!);
            return Ok();
        }
    }
}
