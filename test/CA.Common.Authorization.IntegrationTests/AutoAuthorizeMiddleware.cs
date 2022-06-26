using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;

namespace CA.Common.Authorization.IntegrationTests
{
    public class AutoAuthorizeMiddleware
    {
        public const string IDENTITY_ID = "2";

        private readonly RequestDelegate _next;

        public AutoAuthorizeMiddleware(RequestDelegate rd)
        {
            _next = rd;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            var id = IDENTITY_ID;
            // check if the request want to use a specific user id
            if (httpContext.Request.Headers.ContainsKey("x-userid"))
            {
                id = httpContext.Request.Headers["x-userid"];
            }

            var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);

            identity.AddClaim(new Claim("sub", id));
            identity.AddClaim(new Claim("unique_name", id));

            httpContext.User = new ClaimsPrincipal(identity);

            await _next.Invoke(httpContext);
        }
    }
}
