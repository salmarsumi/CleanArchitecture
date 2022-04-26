using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace CA.MediatR.Behaviors
{
    public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;

        public LoggingBehavior(
            IHttpContextAccessor httpContextAccessor,
            ILogger<LoggingBehavior<TRequest, TResponse>> logger)
        {
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        public Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            const string requestIdHeader = "Request-Id";
            var correlationId = string.Empty;
            var userId = string.Empty;
            var username = string.Empty;

            if (_httpContextAccessor.HttpContext.Request.Headers.ContainsKey(requestIdHeader))
            {
                correlationId = _httpContextAccessor.HttpContext.Request.Headers[requestIdHeader].First();
            }

            userId = _httpContextAccessor.HttpContext.User.HasClaim(x => x.Type == "sub") ? _httpContextAccessor.HttpContext.User.FindFirst("sub")!.Value : "User id not found";
            username = _httpContextAccessor.HttpContext.User.HasClaim(x => x.Type == "email") ? _httpContextAccessor.HttpContext.User.FindFirst("email")!.Value : "Username not found";
            
            using (_logger.BeginScope("{CorrelationId} {UserId} {Username} {RequestHost} {RequestScheme}",
                correlationId,
                userId,
                username,
                _httpContextAccessor.HttpContext.Request.Host.Value,
                _httpContextAccessor.HttpContext.Request.Scheme))
            {
                _logger.LogInformation(
                "----- Handling Request {RequestName}: ({@Command})",
                request.GetGenericTypeName(),
                // check if a safe copy is needed before logging the request
                request is ISecuritySensitive<TRequest> sensitive ? sensitive.GetSafeCopy() : request);

                return next();
            }
        }
    }
}
