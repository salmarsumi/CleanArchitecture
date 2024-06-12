using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace CA.Common.Services
{
    /// <summary>
    /// Current request information operations implementation.
    /// </summary>
    public class CurrentRequestService : ICurrentRequestService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentRequestService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public string GetClientBrowser()
        {
            return _httpContextAccessor.HttpContext.Request.Headers["User-Agent"].FirstOrDefault();
        }

        public string GetClientIPAddress()
        {
            return _httpContextAccessor.HttpContext.Connection.RemoteIpAddress?.ToString();
        }

        public string GetCorrelationId()
        {
            if (_httpContextAccessor.HttpContext.Request.Headers.ContainsKey(Constants.CORRELATION_HEADER))
            {
                return _httpContextAccessor.HttpContext.Request.Headers[Constants.CORRELATION_HEADER].First();
            }

            return string.Empty;
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
            if (_httpContextAccessor.HttpContext.User.Identity.IsAuthenticated)
            {
                return _httpContextAccessor.HttpContext.User.Identity.Name;
            }
            return "Not Authenticated";
        }

        public bool IsAuthenticated()
        {
            return _httpContextAccessor.HttpContext.User.Identity.IsAuthenticated;
        }
    }
}
