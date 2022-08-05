using CA.Common.Authorization.PolicyRuntime;
using System.Security.Claims;

namespace CA.Common.Authorization.Client
{
    public class PolicyOperations : IPolicyOperations
    {
        private static readonly Policy _policy = new(
            new Group[]
            {
                new()
                {
                    Name = "Admin",
                    Users = new[] { "2" }
                },
            },
            new Permission[]
            {
                new()
                {
                    Name = AppPermissions.ViewWeather,
                    Groups = new[] { "Admin" }
                },
                new()
                {
                    Name = AppPermissions.CreateWeather,
                    Groups = new[] { "Admin" }
                },
                new()
                {
                    Name = AppPermissions.DeleteWeather,
                    Groups = new[] { "Admin" }
                },
                new()
                {
                    Name = AppPermissions.ViewAudit,
                    Groups = new[] { "Admin" }
                },
                new()
                {
                    Name = AppPermissions.ViewAccess,
                    Groups = new[] { "Admin" }
                }
            });

        public Task<PolicyEvaluationResult> EvaluateAsync(ClaimsPrincipal user)
        {
            return _policy.EvaluateAsync(user);
        }

        public async Task<bool> HasPermissionAsync(ClaimsPrincipal user, string permission)
        {
            var policy = await _policy.EvaluateAsync(user);
            return policy.Permissions.Contains(permission);
        }

        public async Task<bool> IsInGroupAsync(ClaimsPrincipal user, string group)
        {
            var policy = await _policy.EvaluateAsync(user);
            return policy.Groups.Contains(group);
        }
    }
}
