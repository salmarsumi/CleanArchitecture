using SMD.Security.Authorization.Client;
using System.Security.Claims;

namespace CA.Authorization.Endpoints
{
    public static class PolicyEndpoints
    {
        public static WebApplication MapPolicyEndpoints(this WebApplication app)
        {
            app.MapGet("/policy", async (IPolicyOperations policyClient, ClaimsPrincipal user) =>
            {
                var result = await policyClient.EvaluateAsync(user);

                return Results.Ok(result);
            }).RequireAuthorization();

            return app;
        }
    }
}
