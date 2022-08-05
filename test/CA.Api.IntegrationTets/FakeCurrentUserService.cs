using CA.Common.Services;
using System.Security.Claims;

namespace CA.Api.IntegrationTets
{
    public class FakeCurrentUserService : ICurrentRequestService
    {
        public string GetClientBrowser()
        {
            throw new NotImplementedException();
        }

        public string GetClientIPAddress()
        {
            throw new NotImplementedException();
        }

        public string GetCorrelationId()
        {
            throw new NotImplementedException();
        }

        public ClaimsPrincipal GetUser()
        {
            throw new NotImplementedException();
        }

        public string GetUserId()
        {
            return Guid.NewGuid().ToString();
        }

        public string GetUsername()
        {
            return "Fake Username";
        }

        public bool IsAuthenticated()
        {
            return true;
        }
    }
}
