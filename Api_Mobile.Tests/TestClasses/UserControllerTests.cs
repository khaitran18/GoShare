using Application.Common.Dtos;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Xunit;
using Application.Queries;

namespace Api_Mobile.Tests.TestClasses
{
    public class UserControllerTests : IClassFixture<CustomWebApplicationFactory>, IDisposable
    {
        private readonly CustomWebApplicationFactory _factory;
        private readonly HttpClient _client;
        //private readonly MockServer _mockServer;
        //private bool _disposed;

        public UserControllerTests(CustomWebApplicationFactory factory)
        {
            _factory = factory;
            _client = _factory.CreateClient();
            //_mockServer = new MockServer();
        }

        public void Dispose()
        {
            _client.Dispose();
            _factory.Dispose();
        }

        [Fact]
        public async Task GetDependents_ReturnsOkResult_WithDependents()
        {
            // Arrange
            //var token = await _mockServer.AuthenticateAsync();
            //_client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            string requestJson = File.ReadAllText("../RequestData/CreateTripRequest.json");
            var command = JsonConvert.DeserializeObject<GetDependentsQuery>(requestJson);

            // Act
            var response = await _client.GetAsync("/User/dependents");

            // Assert
            response.EnsureSuccessStatusCode(); // Status Code 200-299
            var actualDependents = JsonConvert.DeserializeObject<List<UserDto>>(
                await response.Content.ReadAsStringAsync());

            actualDependents.Should().NotBeNull();
        }

        //protected virtual void Dispose(bool disposing)
        //{
        //    if (!_disposed)
        //    {
        //        if (disposing)
        //        {
        //            _client.Dispose();
        //            _mockServer.Dispose();
        //        }

        //        _disposed = true;
        //    }
        //}

        //~UserControllerTests()
        //{
        //    Dispose(disposing: false);
        //}

        //public void Dispose()
        //{
        //    Dispose(disposing: true);
        //    GC.SuppressFinalize(this);
        //}
    }
}
