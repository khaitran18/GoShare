using Application.Commands;
using Application.Common.Dtos;
using Domain.Enumerations;
using Moq;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Api_Mobile.Tests.TestClasses
{
    public class TripControllerTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly CustomWebApplicationFactory _factory;
        private readonly HttpClient _client;

        public TripControllerTests(CustomWebApplicationFactory factory)
        {
            _factory = factory;
            _client = _factory.CreateClient();
        }

        [Fact]
        public async Task CreateTrip_ReturnsOkResult_WithTripId()
        {
            // Arrange
            var expectedToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCIsImN0eSI6IkpXVCJ9.eyJpZCI6IjUzMmQ3M2NhLWFjNWQtNDczMi1hNDhiLTc1MDUwMmQxOWMzNyIsInBob25lIjoiODQ5MzM2ODQ5MDkiLCJuYW1lIjoiVGhvIE5ndXllbiIsImh0dHA6Ly9zY2hlbWFzLm1pY3Jvc29mdC5jb20vd3MvMjAwOC8wNi9pZGVudGl0eS9jbGFpbXMvcm9sZSI6IlVzZXIiLCJleHAiOjE3MDAyMjM2MjgsImlzcyI6Imp3dCIsImF1ZCI6Imp3dCJ9.V384kol-2Is-585pTJBDNGkTax9hyD77LKaWGtpKVK0";
            var expectedPrincipal = new ClaimsPrincipal();

            _factory.TokenServiceMock
                .Setup(t => t.ValidateToken(It.IsAny<string>()))
                .Returns(expectedPrincipal);

            _factory.TokenServiceMock
                .Setup(t => t.GenerateJWTToken(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<UserRoleEnumerations>()))
                .Returns(expectedToken);

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", expectedToken);

            string path = Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "..", "TestData", "CreateTripData.json");
            string createTripCommand = File.ReadAllText(path);

            var expectedTrip = new TripDto { Id = Guid.NewGuid() };

            _factory.MediatorMock
                .Setup(m => m.Send(It.IsAny<CreateTripCommand>(), It.IsAny<CancellationToken>()))!
                .ReturnsAsync(expectedTrip);

            var content = new StringContent(createTripCommand, Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync("/api/Trip", content);

            // Assert
            response.EnsureSuccessStatusCode(); // Status Code 200-299
            var actualTrip = JsonConvert.DeserializeObject<TripDto>(
                await response.Content.ReadAsStringAsync());

            Assert.Equal(expectedTrip.Id, actualTrip!.Id);
        }

        //[Fact]
        //public void CreateTripCommandValidator_ShouldHaveError_WhenCartypeIdIsNull()
        //{
        //    // Arrange
        //    string path = Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "..", "TestData", "CreateTripData.json");
        //    string data = File.ReadAllText(path);
        //    var createTripCommand = JsonConvert.DeserializeObject<CreateTripCommand>(data);

        //    var validator = new CreateTripCommandValidator();

        //    // Act
        //    var result = validator.Validate(createTripCommand!);

        //    // Assert
        //    result.IsValid.Should().BeFalse();
        //    result.Errors.Should().Contain(e => e.PropertyName == "CartypeId");
        //}

        [Fact]
        public async Task CreateTrip_ReturnsUnauthorized_WhenNoToken()
        {
            // Arrange
            string path = Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "..", "TestData", "CreateTripData.json");
            string createTripCommand = File.ReadAllText(path);

            var content = new StringContent(createTripCommand, Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync("/api/Trip", content);

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }
    }
}
