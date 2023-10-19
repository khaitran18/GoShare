using Microsoft.Extensions.Configuration;

namespace Application.Configuration
{
    public static class GoShareConfiguration
    {
        #region Private Members to get configuration
        private static IConfiguration? configuration;

        private static IConfiguration Configuration
        {
            get
            {
                if (configuration == null)
                {
                    throw new Exception("Configuration has not been initialized yet!");
                }
                return configuration;
            }
        }

        #endregion

        #region Initialize the configuration

        public static void Initialize(IConfiguration _configuration)
        {
            configuration = _configuration;
        }

        #endregion

        #region Public Configuration Fields

        public static string ConnectionString(string connectionStringKey)
        {
            Console.WriteLine("Add connection string");
            Console.WriteLine(Configuration.GetConnectionString(connectionStringKey)!);
            return Configuration.GetConnectionString(connectionStringKey)!;
        }

        //public static string ValidAudience
        //    => Configuration.GetSection("JWT")["ValidAudience"];

        //public static string ValidIssuer
        //    => Configuration.GetSection("JWT")["ValidIssuer"];

        //public static string Secret
        //    => Configuration.GetSection("JWT")["Secret"];

        #endregion

        #region Firebase
        public static string FirebaseCredentialFile
            => Configuration["Google:Firebase:CredentialFile"]!;

        public static string FirebaseProjectId
            => Configuration["Google:Firebase:ProjectId"]!;

        //public static string FirebaseApiKey
        //    => Configuration["Google:Firebase:ApiKey"];
        #endregion

        #region Google Maps
        public static string GoogleMapsApi
            => Configuration["Google:Maps:ApiKey"]!;
        #endregion

        #region
        public static Twilio TwilioAccount => new Twilio
        {
            AccountSid = Configuration["Twilio:AccountSid"]!,
            AuthToken = Configuration["Twilio:AuthToken"]!,
            VerificationSid = Configuration["Twilio:VerificationSid"]!
        };
        #endregion

        public static SpeedSMS SpeedSMSAccount => new SpeedSMS
        {
            AccessToken = Configuration["SpeedSMSAPI:AccessToken"]!,
            DeviceId = Configuration["SpeedSMSAPI:DeviceId"]!
        };
        public static Jwt jwt => new Jwt
        {
            key = Configuration["Jwt:Key"]!,
            expiryTime = Configuration["Jwt:ExpiryMinutes"]!,
            refreshTokenExpiryTime = Configuration["Jwt:RefreshTokenExpiryMinutes"]!,
            issuer = Configuration["Jwt:Issuer"]!,
            audience = Configuration["Jwt:Audience"]!
        };
    }
}
