﻿using Application.Common.Exceptions;

namespace Api_Mobile.Middlewares
{
    public class ExceptionHandlingMiddleware : IMiddleware
    {
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                await next(context);
            }
            catch (Exception ex)
            {
                if (ex is NotFoundException)
                {
                    context.Response.StatusCode = 404;
                    context.Response.ContentType = "application/json";
                    var responseBody = new
                    {
                        Message = ex.Message,
                        StackTrace = ex.StackTrace
                    };
                    await context.Response.WriteAsJsonAsync(responseBody);
                }
                else if (ex is BadRequestException badreq)
                {
                    context.Response.StatusCode = 400;
                    context.Response.ContentType = "application/json";
                    var responseBody = new
                    {
                        Message = ex?.Message,
                        StackTrace = ex?.StackTrace
                    };
                    await context.Response.WriteAsJsonAsync(responseBody);
                }
                else if (ex is ForbiddenAccessException forbid)
                {
                    context.Response.StatusCode = 403;
                    context.Response.ContentType = "application/json";
                    var responseBody = new
                    {
                        Message = ex?.Message,
                        StackTrace = ex?.StackTrace
                    };
                    await context.Response.WriteAsJsonAsync(responseBody);
                }
                else if (ex is ValidationException valid)
                {
                    context.Response.StatusCode = 400;
                    context.Response.ContentType = "application/json";
                    var responseBody = new
                    {
                        Message = valid.Errors.Values.ToArray(),
                        StackTrace = ex.StackTrace
                    };
                    await context.Response.WriteAsJsonAsync(responseBody);
                }
                else
                {
                    context.Response.StatusCode = 500;
                    context.Response.ContentType = "application/json";
                    var responseBody = new
                    {
                        Message = ex.Message,
                        StackTrace = ex.StackTrace
                    };
                    await context.Response.WriteAsJsonAsync(responseBody);
                }
            }
        }
    }
}
