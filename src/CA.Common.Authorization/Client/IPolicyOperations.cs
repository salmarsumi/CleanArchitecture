using CA.Common.Authorization.PolicyRuntime;
using System.Security.Claims;

namespace CA.Common.Authorization.Client
{
    public interface IPolicyOperations
    {
        Task<PolicyEvaluationResult> EvaluateAsync(ClaimsPrincipal user);
        Task<bool> HasPermissionAsync(ClaimsPrincipal user, string permission);
        Task<bool> IsInGroupAsync(ClaimsPrincipal user, string group);
    }
}
