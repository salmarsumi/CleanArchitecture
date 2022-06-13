using System.Security.Claims;

namespace CA.Common.Authorization.PolicyRuntime
{
    public class Policy
    {
        public Policy(IEnumerable<Role> roles, IEnumerable<Permission> permissions)
        {
            Roles = roles ?? throw new ArgumentNullException(nameof(roles));
            Permissions = permissions ?? throw new ArgumentNullException(nameof(permissions));
        }

        public IEnumerable<Role> Roles { get; private set; }

        public IEnumerable<Permission> Permissions { get; private set; }

        internal Task<PolicyEvaluationResult> EvaluateAsync(ClaimsPrincipal user)
        {
            if(user is null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            var roles = Roles
                .Where(r => r.Evaluate(user))
                .Select(r => r.Name);
            var permissions = Permissions
                .Where(p => p.Evaluate(roles))
                .Select(p => p.Name);

            var result = new PolicyEvaluationResult()
            {
                Roles = roles.Distinct(),
                Permissions = permissions.Distinct()
            };

            return Task.FromResult(result);
        }
    } 
}
