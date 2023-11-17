using Application.Common.Dtos;
using Application.Queries;
using Domain.Enumerations;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Moq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Api_Mobile.Tests.TestClasses
{
    public class UserControllerTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly CustomWebApplicationFactory _factory;
        private readonly HttpClient _client;

        public UserControllerTests(CustomWebApplicationFactory factory)
        {
            _factory = factory;
            _client = _factory.CreateClient();
        }

        [Fact]
        public async Task GetDependents_ReturnsOkResult_WithDependents()
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

            string path = Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "..", "TestData", "GetDependentsData.json");
            string data = File.ReadAllText(path);

            var expectedDependents = JsonConvert.DeserializeObject<PaginatedResult<UserDto>>(data);

            _factory.MediatorMock
                .Setup(m => m.Send(It.IsAny<GetDependentsQuery>(), It.IsAny<CancellationToken>()))!
                .ReturnsAsync(expectedDependents);

            // Act
            var response = await _client.GetAsync("/api/User/dependents");

            // Assert
            response.EnsureSuccessStatusCode(); // Status Code 200-299
            var actualDependents = JsonConvert.DeserializeObject<PaginatedResult<UserDto>>(
                await response.Content.ReadAsStringAsync());

            actualDependents.Should().BeEquivalentTo(expectedDependents);
        }

        [Fact]
        public async Task GetDependents_ReturnsUnauthorized_WhenNoToken()
        {
            // Act
            var response = await _client.GetAsync("/api/User/dependents");

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task GetDependents_ReturnsOkResult_WithNoDependents_WhenPageSizeIsInvalid()
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

            var invalidPageSize = -1;
            var url = $"/api/User/dependents?pageSize={invalidPageSize}";

            var expectedDependents = new PaginatedResult<UserDto>(new List<UserDto>(), 0, 1, 10);

            _factory.MediatorMock
                .Setup(m => m.Send(It.IsAny<GetDependentsQuery>(), It.IsAny<CancellationToken>()))!
                .ReturnsAsync(expectedDependents);

            // Act
            var response = await _client.GetAsync(url);

            // Assert
            response.EnsureSuccessStatusCode(); // Status Code 200-299
            var actualDependents = JsonConvert.DeserializeObject<PaginatedResult<UserDto>>(
                await response.Content.ReadAsStringAsync());

            actualDependents.Should().BeEquivalentTo(expectedDependents);
        }

        [Fact]
        public async Task GetDependents_ReturnsOkResult_WithNoDependents_WhenPageIsInvalid()
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

            var invalidPage = -1;
            var url = $"/api/User/dependents?page={invalidPage}";

            var expectedDependents = new PaginatedResult<UserDto>(new List<UserDto>(), 0, 1, 10);

            _factory.MediatorMock
                .Setup(m => m.Send(It.IsAny<GetDependentsQuery>(), It.IsAny<CancellationToken>()))!
                .ReturnsAsync(expectedDependents);

            // Act
            var response = await _client.GetAsync(url);

            // Assert
            response.EnsureSuccessStatusCode(); // Status Code 200-299
            var actualDependents = JsonConvert.DeserializeObject<PaginatedResult<UserDto>>(
                await response.Content.ReadAsStringAsync());

            actualDependents.Should().BeEquivalentTo(expectedDependents);
        }

        [Fact]
        public async Task GetDependents_ReturnsOkResult_WithNoDependents_WhenSortByIsInvalid()
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

            var invalidSortBy = "invalidSortBy";
            var url = $"/api/User/dependents?sortBy={invalidSortBy}";

            var expectedDependents = new PaginatedResult<UserDto>(new List<UserDto>(), 0, 1, 10);

            _factory.MediatorMock
                .Setup(m => m.Send(It.IsAny<GetDependentsQuery>(), It.IsAny<CancellationToken>()))!
                .ReturnsAsync(expectedDependents);

            // Act
            var response = await _client.GetAsync(url);

            // Assert
            response.EnsureSuccessStatusCode(); // Status Code 200-299
            var actualDependents = JsonConvert.DeserializeObject<PaginatedResult<UserDto>>(
                await response.Content.ReadAsStringAsync());

            actualDependents.Should().BeEquivalentTo(expectedDependents);
        }

        [Fact]
        public async Task GetDependents_ReturnsOkResult_WithNoDependents()
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

            var expectedDependents = new PaginatedResult<UserDto>(new List<UserDto>(), 0, 1, 10);

            _factory.MediatorMock
                .Setup(m => m.Send(It.IsAny<GetDependentsQuery>(), It.IsAny<CancellationToken>()))!
                .ReturnsAsync(expectedDependents);

            // Act
            var response = await _client.GetAsync("/api/User/dependents");

            // Assert
            response.EnsureSuccessStatusCode(); // Status Code 200-299
            var actualDependents = JsonConvert.DeserializeObject<PaginatedResult<UserDto>>(
                await response.Content.ReadAsStringAsync());

            actualDependents.Should().BeEquivalentTo(expectedDependents);
        }
    }
}
