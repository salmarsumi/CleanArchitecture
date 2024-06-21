using System.Security.Claims;

namespace CA.Common.Authorization.PolicyRuntime
{
    /// <summary>
    /// Represents the entire policy configuration with all permissions and user groups defined in the system.
    /// This class will be the single source of truth regarding which user can have what permission.Given a user
    /// the policy instance can evaluate and return what permissions and membership the user has.
    /// </summary>
    public class Policy
    {
        public Policy(IEnumerable<Group> groups, IEnumerable<Permission> permissions)
        {
            Groups = groups;
            Permissions = permissions;
        }

        public IEnumerable<Group> Groups { get; private set; }

        public IEnumerable<Permission> Permissions { get; private set; }

        /// <summary>
        /// Evaluate the group membership and permissions assignments for a given user.
        /// </summary>
        /// <param name="user"><The user <see cref="ClaimsPrincipal"/> instance./param>
        /// <returns>The <see cref="PolicyEvaluationResult"/> instance holding the user related groups and permissions.</returns>
        /// <exception cref="ArgumentNullException" />
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
