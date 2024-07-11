using CA.Common.ResponseTypes;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CA.MediatR.Behaviors
{
    /// <summary>
    /// Apply all registered validation rules for the current request.
    /// </summary>
    /// <typeparam name="TRequest">The type of the request being processed.</typeparam>
    /// <typeparam name="TResponse">The type of the response generated buy tje current request.</typeparam>
    public class ValidatorBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
        where TResponse : IRequestResult
    {
        private readonly ILogger<ValidatorBehavior<TRequest, TResponse>> _logger;
        private readonly IEnumerable<IValidator<TRequest>> _validators;

        public ValidatorBehavior(IEnumerable<IValidator<TRequest>> validators, ILogger<ValidatorBehavior<TRequest, TResponse>> logger)
        {
            _validators = validators;
            _logger = logger;
        }

        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            var typeName = request?.GetTypeName();

            _logger.LogInformation("----- Validating request {CommandType}", typeName);

            var failures = _validators
                .Select(v => v.Validate(request))
                .SelectMany(result => result.Errors)
                .Where(error => error is not null)
                .ToList();

            if (failures.Any())
            {
                _logger.LogWarning("----- Validation errors - {CommandType} - Command: {@Command} - Errors: {@ValidationErrors}", typeName, request, failures);


                return (TResponse)TResponse.NotValid(new JsonErrorResponse
                {
                    ExceptionType = "Validation",
                    Data = failures?.Select(error => error.ErrorMessage)?.ToArray()
                });
            }

            return await next();
        }
    }
}
