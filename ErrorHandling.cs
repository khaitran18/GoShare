using Microsoft.AspNetCore.Mvc;
using Application.Common.Exceptions;
using Application.Common;

namespace Api_Mobile
{
    public class ErrorHandling<TResponse> : IActionResult
    {
        private readonly BaseResponse<TResponse> _exception;

        public ErrorHandling(BaseResponse<TResponse> e)
        {
            _exception = e;
        }

        public async Task ExecuteResultAsync(ActionContext context)
        {
            var objectResult = new ObjectResult(new
            {
                Message = _exception.Message,
                StatusCode = 500 // Set the desired HTTP status code
            });
            if (_exception.Exception is BadRequestException badreq)
            {
                objectResult.StatusCode = 400;
                objectResult.Value = _exception.Message;
            }
            if (_exception.Exception is ForbiddenAccessException forbid)
            {
                objectResult.StatusCode = 403;
                objectResult.Value = _exception.Message;
            }
            if (_exception.Exception is NotFoundException notfound)
            {
                objectResult.StatusCode = 404;
                objectResult.Value = _exception.Message;
            }
            if (_exception.Exception is ValidationException validate)
            {
                objectResult.StatusCode = 400;
                objectResult.Value = _exception.Message;
            }
            if (_exception.Message == null) objectResult.Value = _exception.Exception.Message;
            await objectResult.ExecuteResultAsync(context);
        }
    }
}