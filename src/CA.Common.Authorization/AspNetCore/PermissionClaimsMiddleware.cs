using CA.Common.Authorization.Client;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace CA.Common.Authorization.AspNetCore
{
    /// <summary>
    /// Convert user roles and permissions into identity claims
    /// </summary>
    public class PermissionClaimsMiddleware
    {
        private readonly RequestDelegate _next;

        public PermissionClaimsMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context, IPolicyOperations client)
        {
            if (context.User.Identity.IsAuthenticated)
            {
                // The evaluate call will either be remote or local depending on the 
                // registered IPolicyOperations implementation in the DI container
                var policy = await client.EvaluateAsync(context.User);

                if (policy != null)
                {
                    var roleClaims = policy.Roles?.Select(x => new Claim("role", x));
                    var permissionClaims = policy.Permissions?.Select(x => new Claim("permission", x));

                    if (roleClaims != null)
                    {
                        var id = new ClaimsIdentity("PolicyClaimsIdentity", "name", "role");
                        id.AddClaims(roleClaims);

                        if (permissionClaims != null)
                            id.AddClaims(permissionClaims);

                        context.User.AddIdentity(id);
                    }
                }
            }

            await _next(context);
        }
    }
}
