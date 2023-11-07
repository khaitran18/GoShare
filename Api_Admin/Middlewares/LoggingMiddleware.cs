namespace Api_Admin.Middlewares
{
    public class LoggingMiddleware : IMiddleware
    {
        private readonly ILogger _logger;

        public LoggingMiddleware(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<LoggingMiddleware>();
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            _logger.LogInformation("Request start:"+DateTime.Now);
            _logger.LogInformation("Path:" + context.Request.Path);
            await next(context);
            _logger.LogInformation("Request end:" + DateTime.Now);
        }
    }
}
