using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace CA.Common.Services
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public ClaimsPrincipal GetUser()
        {
            return _httpContextAccessor.HttpContext.User;
        }

        public string GetUserId()
        {
            return _httpContextAccessor.HttpContext.User.FindFirst("sub")?.Value ?? "not found";
        }

        public string GetUsername()
        {
            return _httpContextAccessor.HttpContext.User.Identity.Name ?? "Not Authenticated";
        }

        public bool IsAuthenticated()
        {
            return _httpContextAccessor.HttpContext.User.Identity.IsAuthenticated;
        }
    }
}
