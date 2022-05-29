using System.Security.Claims;

namespace CA.WebAngular.Endpoints
{
    public static class AccountEndpoints
    {
        public static WebApplication MapAccountEndpoints(this WebApplication app)
        {
            app.MapGet("/account/session", (ClaimsPrincipal user) =>
            {
                object result = null;

                if (user.Identity.IsAuthenticated)
                {
                    result = new { username = user.Identity.Name };
                }

                return Results.Ok(result);
            });

            app.MapGet("/account/postlogin", () => Results.LocalRedirect("/"));

            return app;
        }
    }
}
