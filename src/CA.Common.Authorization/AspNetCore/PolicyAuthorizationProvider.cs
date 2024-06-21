using CA.Common.Authorization.Client;
using CA.Common.Contracts.Audit;
using CA.Common.Services;
using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace CA.Common.Authorization.AspNetCore
{
    /// <summary>
    /// This provider will automatically create ASP.NET Core authorization policies for all permissions used in the application.
    /// </summary>
    public class PolicyAuthorizationProvider : DefaultAuthorizationPolicyProvider
    {
        public PolicyAuthorizationProvider(IOptions<AuthorizationOptions> options)
            : base(options)
        { }

        public override async Task<AuthorizationPolicy> GetPolicyAsync(string policyName)
        {
            var policy = await base.GetPolicyAsync(policyName);

            if (policy == null)
            {
                policy = new AuthorizationPolicyBuilder()
                    .AddRequirements(new PermissionRequirement(policyName))
                    .Build();
            }

            return policy;
        }
    }

    /// <summary>
    /// Represent the requirement needed to pass the authorization check, 
    /// this will be the name of the required permission.
    /// </summary>
    public class PermissionRequirement : IAuthorizationRequirement
    {
        public PermissionRequirement(string name)
        {
            Name = name;
        }

        public string Name { get; set; }
    }

    /// <summary>
    /// Extends the AuthorizationHandler class to integrate with the ASP.NET Core authorization.
    /// The PermissionHandler will provide ASP.NET Core authorization the means to check if a 
    /// specific user has a required permission through the use of <see cref="IPolicyOperations"/>.
    /// </summary>
    public class PermissionHandler : AuthorizationHandler<PermissionRequirement>
    {
        private readonly IPolicyOperations _client;
        private readonly ILogger<PermissionHandler> _logger;
        private readonly ICurrentRequestService _currentRequest;
        private readonly IPublishEndpoint _publishEndpoint;

        public PermissionHandler(IPolicyOperations client, ILogger<PermissionHandler> logger, ICurrentRequestService currentRequest, IPublishEndpoint publishEndpoint = null)
        {
            _client = client;
            _logger = logger;
            _currentRequest = currentRequest;
            _publishEndpoint = publishEndpoint;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
        {
            if (await _client.HasPermissionAsync(context.User, requirement.Name))
            {
                context.Succeed(requirement);
            }
            else
            {
                _logger.LogWarning("----- Permission Authorization Failed {Permission}", requirement.Name);
                
                // Publish an AccessEntry event for audit when the authorization fails.
                if(_publishEndpoint is not null)
                {
                    await _publishEndpoint.Publish<AccessEntry>(new
                    {
                        CorrelationId = _currentRequest.GetCorrelationId(),
                        UserId = _currentRequest.GetUserId(),
                        Username = _currentRequest.GetUsername(),
                        Action = "Permission Authorization",
                        Result = "Forbidden",
                        Details = $"Access denied for permission {requirement.Name}",
                        Browser = _currentRequest.GetClientBrowser(),
                        IPAddress = _currentRequest.GetClientIPAddress(),
                        TimeStamp = DateTime.UtcNow
                    });
                }
            }
        }
    }
}
