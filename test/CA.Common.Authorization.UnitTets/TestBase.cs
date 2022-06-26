using System.Security.Claims;

namespace SMD.Security.Authorization.UnitTests
{
    public class TestBase
    {
        protected ClaimsPrincipal SetupUser(Claim subClaim = null)
        {
            var id = new ClaimsIdentity("Test", "name", "role");

            if (!(subClaim is null))
            {
                id.AddClaim(subClaim);
            }

            var user = new ClaimsPrincipal();
            user.AddIdentity(id);
            return user;
        }
    }
}
