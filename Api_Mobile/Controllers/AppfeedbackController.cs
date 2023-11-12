﻿using Application.Common;
using Application.Common.Dtos;
using Application.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Api_Mobile.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AppfeedbackController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AppfeedbackController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetAppfeedbacks([FromQuery] GetAppfeedbacksQuery query)
        {
            var response = await _mediator.Send(query);
            return Ok(response);
        }
    }
}
