namespace Application.Configuration
{
    public class Twilio
    {
        public string AccountSid { get; set; } = "";
        public string AuthToken { get; set; } = "";
        public string VerificationSid { get; set; } = "";
        public short OtpLifeSpan;
    }
}
