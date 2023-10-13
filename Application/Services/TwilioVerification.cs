using Domain.DataModels;
using Domain.Interfaces;
using Twilio;
using Twilio.Exceptions;
using Twilio.Rest.Verify.V2.Service;

namespace Application.Services
{
    public interface ITwilioVerification
    {
        Task<VerificationResult> StartVerificationAsync(string phoneNumber, string channel);

        Task<VerificationResult> CheckVerificationAsync(string phoneNumber, string code);
    }
    public class TwilioVerification : ITwilioVerification
    {
        private readonly Configuration.Twilio _config;

        public TwilioVerification(Configuration.Twilio configuration)
        {
            _config = configuration;
            TwilioClient.Init(_config.AccountSid, _config.AuthToken);
        }

        public async Task<VerificationResult> StartVerificationAsync(string phoneNumber, string channel)
        {
            var verificationResource = await VerificationResource.CreateAsync(
                to: phoneNumber,
                channel: channel,
                pathServiceSid: _config.VerificationSid
            );
            return new VerificationResult(verificationResource.Sid);
        }

        public async Task<VerificationResult> CheckVerificationAsync(string phoneNumber, string code)
        {
            var verificationCheckResource = await VerificationCheckResource.CreateAsync(
                to: phoneNumber,
                code: code,
                pathServiceSid: _config.VerificationSid
            );
            return verificationCheckResource.Status.Equals("approved") ?
                new VerificationResult(verificationCheckResource.Sid) :
                new VerificationResult(new List<string> { "Wrong code. Try again." });
        }
    }
}
