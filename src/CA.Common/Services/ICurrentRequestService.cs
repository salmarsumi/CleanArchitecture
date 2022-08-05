using System.Security.Claims;

namespace CA.Common.Services
{
    public interface ICurrentRequestService
    {
        string GetUserId();
        string GetUsername();
        bool IsAuthenticated();
        ClaimsPrincipal GetUser();
        string GetCorrelationId();
        string GetClientIPAddress();
        string GetClientBrowser();
    }
}
