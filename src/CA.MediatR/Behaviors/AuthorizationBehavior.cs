using CA.Common.Exceptions;
using CA.Common.Services;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SMD.Security.Authorization.Client;

namespace CA.MediatR.Behaviors
{
    public class AuthorizationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ICurrentUserService _currentUserService;
        private readonly ILogger<AuthorizationBehavior<TRequest, TResponse>> _logger;

        public AuthorizationBehavior(IHttpContextAccessor httpContextAccessor, ICurrentUserService currentUserService, ILogger<AuthorizationBehavior<TRequest, TResponse>> logger)
        {
            _httpContextAccessor = httpContextAccessor;
            _currentUserService = currentUserService;
            _logger = logger;
        }

        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            if(request is IAuthorizedRequest authorizedRequest)
            {
                // User needs to be authenticated
                if (!_currentUserService.IsAuthenticated())
                {
                    throw new UnauthorizedAccessException();
                }

                var requiredPermissions = authorizedRequest.GetRequiredPermissions();
                if (requiredPermissions?.Any() == true)
                {
                    var policyOperations = _httpContextAccessor.HttpContext.RequestServices.GetRequiredService<IPolicyOperations>();
                    foreach(var permission in requiredPermissions)
                    {
                        if(! await policyOperations.HasPermissionAsync(_currentUserService.GetUser(), permission))
                        {
                            _logger.LogWarning("----- Permission Authorization Failed {Permission}", permission);
                            throw new ForbiddenAccessException();
                        }
                    }
                }
            }

            // if we reach this point either the user is authorized or does not require authorization
            return await next();
        }
    }
}
