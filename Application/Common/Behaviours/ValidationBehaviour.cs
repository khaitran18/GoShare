using FluentValidation;
using MediatR;
namespace Application.Common.Behaviours
{
    public class ValidationBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    {
        private readonly IEnumerable<IValidator<TRequest>> _validators;
        public ValidationBehaviour(IEnumerable<IValidator<TRequest>> validators) => _validators = validators;

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            var validationFailures = _validators
            .Select(validator => validator.Validate(request))
            .SelectMany(validationResult => validationResult.Errors)
            .Where(validationFailure => validationFailure != null)
            .ToList();

            if (validationFailures.Any())
            {
                //var error = string.Join("\r\n", validationFailures);
                throw new Exceptions.ValidationException(validationFailures);
                //Exceptions.ValidationException exception = new Exceptions.ValidationException();
                //return (TResponse)Activator.CreateInstance(typeof(TResponse), true, error, exception);
            }
            return await next();
        }
    }
}
