﻿using Application.UseCase.DriverUC.Commands;
using Application.UseCase.DriverUC.Queries;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Api_Mobile.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Driver")]
    public class DriverController : ControllerBase
    {
        private readonly IMediator _mediator;
        public DriverController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetDriverInformation()
        {
            GetDriverInformationQuery query = new GetDriverInformationQuery();
            var response = await _mediator.Send(query);
            return Ok(response);
        }

        [HttpPost("activate")]
        public async Task<IActionResult> DriverActivate([FromBody] DriverActivateCommand command)
        {
            var response = await _mediator.Send(command);
            return Ok(response);
        }

        [HttpPost("deactivate")]
        public async Task<IActionResult> DriverDeactivate([FromBody] DriverDeactivateCommand command)
        {
            var response = await _mediator.Send(command);
            return Ok(response);
        }

        [HttpPost("confirm-passenger/{id}")]
        public async Task<IActionResult> ConfirmPassenger([FromBody] ConfirmPassengerCommand command, [FromRoute] Guid id)
        {
            command.TripId = id;
            var response = await _mediator.Send(command);
            return Ok(response);
        }

        [HttpPost("confirm-pickup/{id}")]
        public async Task<IActionResult> ConfirmPickupPassenger([FromForm] ConfirmPickupPassengerCommand command, [FromRoute] Guid id)
        {
            command.TripId = id;
            var response = await _mediator.Send(command);
            return Ok(response);
        }

        [HttpPost("end-trip/{id}")]
        public async Task<IActionResult> EndTrip([FromForm] EndTripCommand command, [FromRoute] Guid id)
        {
            command.TripId = id;
            var response = await _mediator.Send(command);
            return Ok(response);
        }

        [HttpPut("update-location")]
        public async Task<IActionResult> DriverUpdateLocation([FromBody] DriverUpdateLocationCommand command)
        {
            var response = await _mediator.Send(command);
            return Ok(response);
        }

        //[HttpPut("documents")]
        //public async Task<IActionResult> UpdateDriverProfile([FromForm] DriverUpdateDocumentCommand command)
        //{
        //    var response = await _mediator.Send(command);
        //    return Ok(response);
        //}

        [HttpGet("statistic")]
        public async Task<IActionResult> GetStatistic()
        {
            GetDriverWalletStatisticQuery query = new GetDriverWalletStatisticQuery();
            var response = await _mediator.Send(query);
            return Ok(response);
        }
    }
}
