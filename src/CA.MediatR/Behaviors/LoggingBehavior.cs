﻿using CA.Common;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace CA.MediatR.Behaviors
{
    /// <summary>
    /// Logs the request object being handled by the MediatR pipeline.
    /// </summary>
    /// <typeparam name="TRequest">The type of the request being processed.</typeparam>
    /// <typeparam name="TResponse">The type of the response generated buy tje current request.</typeparam>
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

        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            var correlationId = string.Empty;
            var userId = string.Empty;
            var username = string.Empty;

            if (_httpContextAccessor.HttpContext.Request.Headers.ContainsKey(Constants.CORRELATION_HEADER))
            {
                correlationId = _httpContextAccessor.HttpContext.Request.Headers[Constants.CORRELATION_HEADER].First();
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
                request.GetTypeName(),
                // check if a safe copy is needed before logging the request
                request is ISecuritySensitive<TRequest> sensitive ? sensitive.GetSafeCopy() : request);

                return await next();
            }
        }
    }
}
