using CA.Common.Exceptions;
using CA.Common.Services;
using MediatR;
using SMD.Security.Authorization.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CA.MediatR.Behaviors
{
    public class AuthorizationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        private readonly IPolicyOperations _policyOperations;
        private readonly ICurrentUserService _currentUserService;

        public AuthorizationBehavior(IPolicyOperations policyOperations, ICurrentUserService currentUserService)
        {
            _policyOperations = policyOperations;
            _currentUserService = currentUserService;
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
                    foreach(var permission in requiredPermissions)
                    {
                        if(! await _policyOperations.HasPermissionAsync(_currentUserService.GetUser(), permission))
                        {
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
