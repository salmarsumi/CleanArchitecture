using System.Security.Claims;

namespace CA.Common.Authorization.PolicyRuntime
{
    public class Role
    {
        public string Name { get; set; }
        public IEnumerable<string> Subjects { get; set; }

        internal bool Evaluate(ClaimsPrincipal user)
        {
            if(user is null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            var sub = user.FindFirst("sub")?.Value;

            if (!string.IsNullOrEmpty(sub))
            {
                if(Subjects.Contains(sub))
                {
                    return true;
                }
            }

            return false;
        }
    }
}