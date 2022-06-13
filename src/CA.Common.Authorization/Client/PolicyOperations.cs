using CA.Common.Authorization.PolicyRuntime;
using System.Security.Claims;

namespace CA.Common.Authorization.Client
{
    public class PolicyOperations : IPolicyOperations
    {
        private readonly Policy _policy;

        public PolicyOperations(Policy policy)
        {
            _policy = policy;
        }

        public Task<PolicyEvaluationResult> EvaluateAsync(ClaimsPrincipal user)
        {
            return _policy.EvaluateAsync(user);
        }

        public async Task<bool> HasPermissionAsync(ClaimsPrincipal user, string permission)
        {
            var policy = await _policy.EvaluateAsync(user);
            return policy.Permissions.Contains(permission);
        }

        public async Task<bool> IsInRoleAsync(ClaimsPrincipal user, string role)
        {
            var policy = await _policy.EvaluateAsync(user);
            return policy.Roles.Contains(role);
        }
    }
}
