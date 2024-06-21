using CA.Common.Authorization.PolicyRuntime;
using System.Security.Claims;

namespace CA.Common.Authorization.Client
{
    /// <summary>
    /// Defines the operations needed to evaluate users against a policy instance.
    /// </summary>
    public interface IPolicyOperations
    {
        /// <summary>
        /// Evaluate the current group membership and permissions assignment for a given user.
        /// </summary>
        /// <param name="user">The user <see cref="ClaimsPrincipal"/> instance to be evaluated.</param>
        /// <returns></returns>
        Task<PolicyEvaluationResult> EvaluateAsync(ClaimsPrincipal user);

        /// <summary>
        /// Check if a given user has a specific permission.
        /// </summary>
        /// <param name="user">The user <see cref="ClaimsPrincipal"/> instance to be evaluated.</param>
        /// <param name="permission">The name of the permission.</param>
        /// <returns></returns>
        Task<bool> HasPermissionAsync(ClaimsPrincipal user, string permission);

        /// <summary>
        /// Check if a given user is a member of a specific group.
        /// </summary>
        /// <param name="user">The user <see cref="ClaimsPrincipal"/> instance to be evaluated.</param>
        /// <param name="group">The name of the group.</param>
        /// <returns>The <see cref="bool"/> value indicating whether the user is a member of the given group.</returns>
        Task<bool> IsInGroupAsync(ClaimsPrincipal user, string group);
    }
}
