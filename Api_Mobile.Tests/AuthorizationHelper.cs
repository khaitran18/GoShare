using Domain.Enumerations;
using Moq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Api_Mobile.Tests
{
    public class AuthorizationHelper
    {
        private readonly CustomWebApplicationFactory _factory;

        public AuthorizationHelper(CustomWebApplicationFactory factory)
        {
            _factory = factory;
        }

        public void ApplyAuthorization(HttpClient client)
        {
            var expectedToken = GetTokenFromFile();
            var expectedPrincipal = new ClaimsPrincipal();

            _factory.TokenServiceMock
                .Setup(t => t.ValidateToken(It.IsAny<string>()))
                .Returns(expectedPrincipal);

            _factory.TokenServiceMock
                .Setup(t => t.GenerateJWTToken(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<UserRoleEnumerations>()))
                .Returns(expectedToken);

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", expectedToken);
        }

        private string GetTokenFromFile()
        {
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "..", "TestData", "TokenData.json");
            var tokenData = File.ReadAllText(filePath);
            var token = JsonConvert.DeserializeObject<Token>(tokenData);
            return token!.Value;
        }

        public class Token
        {
            public string Value { get; set; } = null!;
        }
    }
}
