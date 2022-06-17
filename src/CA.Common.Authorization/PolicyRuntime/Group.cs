using System.Security.Claims;

namespace CA.Common.Authorization.PolicyRuntime
{
    public class Group
    {
        public string Name { get; set; }
        public IEnumerable<string> Users { get; set; }

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