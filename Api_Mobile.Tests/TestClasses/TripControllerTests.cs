using Api_Mobile.Controllers;
using Application.Commands;
using Application.Common.Dtos;
using AutoFixture.Xunit2;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Api_Mobile.Tests.TestClasses
{
    public class TripControllerTests
    {
        private readonly Mock<IMediator> _mediator;
        private readonly TripController _controller;

        public TripControllerTests()
        {
            _mediator = new Mock<IMediator>();
            _controller = new TripController(_mediator.Object);
        }

        [Fact]
        public async Task CreateTrip_ReturnsOkResult_WhenTripIsCreated()
        {
            // Arrange
            string requestJson = File.ReadAllText("../RequestData/CreateTripRequest.json");
            var command = JsonConvert.DeserializeObject<CreateTripCommand>(requestJson);
            string responseJson = File.ReadAllText("../ResponseData/CreateTripResponse.json");
            var response = JsonConvert.DeserializeObject<TripDto>(responseJson);
            _mediator.Setup(m => m.Send(command!, default))!.ReturnsAsync(response);

            // Act
            var result = await _controller.CreateTrip(command!);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<TripDto>(okResult.Value);
            Assert.Equal(response, returnValue);
        }
    }
}
