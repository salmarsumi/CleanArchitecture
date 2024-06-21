using System.Security.Claims;

namespace CA.Common.Authorization.PolicyRuntime
{
    /// <summary>
    /// Represents a single users group in the system with all the users 
    /// that are members of that specific group. 
    /// Given a user the group instance can evaluate whether this user 
    /// is a member of the specified group.
    /// </summary>
    public class Group
    {
        public string Name { get; set; }
        public IEnumerable<string> Users { get; set; }

        /// <summary>
        /// Evaluate whether a user is a member of the current group.
        /// </summary>
        /// <param name="user">The user <see cref="ClaimsPrincipal"/> instance.</param>
        /// <returns>The <see cref="bool"/> value indicating whether the user is a member of the given group.</returns>
        /// <exception cref="ArgumentNullException" />
        internal bool Evaluate(ClaimsPrincipal user)
        {
            if(user is null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            var sub = user.FindFirst("sub")?.Value;

            if (!string.IsNullOrEmpty(sub))
            {
                if(Users.Contains(sub))
                {
                    return true;
                }
            }

            return false;
        }
    }
}