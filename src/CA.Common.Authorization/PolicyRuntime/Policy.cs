using System.Security.Claims;

namespace CA.Common.Authorization.PolicyRuntime
{
    public class Policy
    {
        public Policy(IEnumerable<Group> groups, IEnumerable<Permission> permissions)
        {
            Groups = groups;
            Permissions = permissions;
        }

        public IEnumerable<Group> Groups { get; private set; }

        public IEnumerable<Permission> Permissions { get; private set; }

        internal Task<PolicyEvaluationResult> EvaluateAsync(ClaimsPrincipal user)
        {
            if(user is null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            var groups = Groups
                .Where(r => r.Evaluate(user))
                .Select(r => r.Name);
            var permissions = Permissions
                .Where(p => p.Evaluate(groups))
                .Select(p => p.Name);

            var result = new PolicyEvaluationResult()
            {
                Groups = groups.Distinct(),
                Permissions = permissions.Distinct()
            };

            return Task.FromResult(result);
        }
    } 
}
