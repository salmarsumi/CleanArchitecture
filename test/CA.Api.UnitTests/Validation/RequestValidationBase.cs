using CA.MediatR;
using CA.MediatR.Behaviors;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using Moq;

namespace CA.Api.UnitTests.Validation
{
    /// <summary>
    /// Base class for validation tests
    /// </summary>
    /// <typeparam name="TRequest">The type of the Request (command or query)</typeparam>
    /// <typeparam name="TResponse">The type of the Response the request should return (command or query)</typeparam>
    /// <typeparam name="TValidator">The type of the validator being tested</typeparam>
    public class RequestValidationBase<TRequest, TResponse, TValidator>
        where TRequest : IRequest<TResponse>
        where TResponse : IRequestResult
        where TValidator : AbstractValidator<TRequest>
    {
        protected readonly ValidatorBehavior<TRequest, TResponse> _validatorBehavior;
        protected readonly RequestHandlerDelegate<TResponse> _next;

        public RequestValidationBase()
        {
            _next = () =>
            {
                return Task.FromResult((TResponse)TResponse.Succeeded());
            };
            var validators = new IValidator<TRequest>[] 
            { 
                Activator.CreateInstance(typeof(TValidator)) as TValidator
            };
            var validatorLogger = new Mock<ILogger<ValidatorBehavior<TRequest, TResponse>>>();

            _validatorBehavior = new ValidatorBehavior<TRequest, TResponse>(validators, validatorLogger.Object);
        }
    }
}
