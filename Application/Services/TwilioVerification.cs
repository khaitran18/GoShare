
using Application.Common.Utilities;
using Application.Services.Interfaces;
using Twilio;
using Twilio.Exceptions;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Rest.Verify.V2.Service;
using Twilio.Types;

namespace Application.Services
{
    public class TwilioVerification : ITwilioVerification
    {
        private readonly Configuration.Twilio _config;
        private readonly string _testNumber = "+84919651361";

        public TwilioVerification(Configuration.Twilio configuration)
        {
            _config = configuration;
            TwilioClient.Init(_config.AccountSid, _config.AuthToken);
        }

        public async Task<VerificationResource> StartVerificationAsync(string phoneNumber, string channel)
        {
            phoneNumber = _testNumber;
            var verificationResource = await VerificationResource.CreateAsync(
                to: phoneNumber,
                channel: channel,
                pathServiceSid: _config.VerificationSid
            );
            // status = pending
            // valid = false
            return verificationResource;
        }

        public async Task<bool> CheckVerificationAsync(string phoneNumber, string code)
        {
            phoneNumber = _testNumber;
            var verificationCheckResource = await VerificationCheckResource.CreateAsync(
                to: phoneNumber,
                code: code,
                pathServiceSid: _config.VerificationSid
            );
            return (verificationCheckResource.Status.Equals("approved")) ? true : false;
            //valid = true || status = approve
            //return verificationCheckResource;
        }

        public Task<string> SendSMS(string phoneNumber, string body)
        {
            phoneNumber = _testNumber;
            var messageOptions = new CreateMessageOptions(new PhoneNumber("+84919651361"));
            messageOptions.From = new PhoneNumber("+17745152793");
            messageOptions.Body = body;
            var message = MessageResource.Create(messageOptions);
            return Task.FromResult(message.Body);
        }

        public Task<DateTime> GenerateOtpExpiryTime()
        {
            return Task.FromResult(DateTimeUtilities.GetDateTimeVnNow().AddMinutes(_config.OtpLifeSpan));
        }
    }
}
