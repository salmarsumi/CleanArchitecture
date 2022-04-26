using System.Security.Claims;

namespace CA.Common.Services
{
    public interface ICurrentUserService
    {
        string GetUserId();
        string GetUsername();
        bool IsAuthenticated();
        ClaimsPrincipal GetUser();
    }
}
