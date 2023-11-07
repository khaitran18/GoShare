using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common.Behaviours
{
    public class LoggingBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> 
        where TRequest : IRequest<TResponse>
    {
        private readonly ILogger<LoggingBehaviour<TRequest, TResponse>> _logger;
        public LoggingBehaviour(ILogger<LoggingBehaviour<TRequest, TResponse>> logger)
        {
            _logger = logger;
        }
        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            //Request
            _logger.LogInformation($"Handling {typeof(TRequest).Name}");
            Type requestType = request!.GetType();
            IList<PropertyInfo> props = new List<PropertyInfo>(requestType.GetProperties());
            foreach (PropertyInfo prop in props)
            {
                object propValue = prop.GetValue(request, null)!;
                _logger.LogInformation("{Property} : {@Value}", prop.Name, propValue);
            }
            var response = await next();
            //Response
            _logger.LogInformation($"Handled {typeof(TResponse).Name}");
            return response;
        }
    }
}
