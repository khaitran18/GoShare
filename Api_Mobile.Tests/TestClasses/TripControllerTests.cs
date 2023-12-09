using Application.Common.Dtos;
using Application.Common.Validations;
using Application.UseCase.TripUC.Commands;
using Domain.Enumerations;
using FluentAssertions;
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
            var authorizationHelper = new AuthorizationHelper(_factory);
            authorizationHelper.ApplyAuthorization(_client);

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

        [Fact]
        public void CreateTripCommandValidator_ShouldHaveError_WhenCartypeIdIsNull()
        {
            // Arrange
            string path = Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "..", "TestData", "CreateTripData.json");
            string data = File.ReadAllText(path);
            var createTripCommand = JsonConvert.DeserializeObject<CreateTripCommand>(data);

            var validator = new CreateTripCommandValidator();

            // Act
            var result = validator.Validate(createTripCommand!);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "CartypeId");
        }

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
