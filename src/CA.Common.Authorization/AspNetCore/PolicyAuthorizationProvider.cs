using CA.Common.Authorization.Client;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace CA.Common.Authorization.AspNetCore
{
    /// <summary>
    /// This provider will automatically create ASP.NET Core authorization policies for all permissions used in the application
    /// </summary>
    public class PolicyAuthorizationProvider : DefaultAuthorizationPolicyProvider
    {
        public PolicyAuthorizationProvider(IOptions<AuthorizationOptions> options)
            : base(options) { }

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

    public class PermissionRequirement : IAuthorizationRequirement
    {
        public PermissionRequirement(string name)
        {
            Name = name;
        }

        public string Name { get; set; }
    }

    public class PermissionHandler : AuthorizationHandler<PermissionRequirement>
    {
        private readonly IPolicyOperations _client;
        private readonly ILogger<PermissionHandler> _logger;

        public PermissionHandler(IPolicyOperations client, ILogger<PermissionHandler> logger)
        {
            _client = client;
            _logger = logger;
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
            }
        }
    }
}
