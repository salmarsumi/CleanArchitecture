using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Serilog;
using Serilog.Events;

namespace CA.Common.Logging
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseCASerilog(this IApplicationBuilder app, IEnumerable<string>? execludedPaths = null)
        {
            app.UseMiddleware<CorrelationIdMiddleware>();

            app.UseSerilogRequestLogging(options =>
            {
                // Path execlusion
                options.GetLevel = (HttpContext ctx, double _, Exception ex) =>
                {
                    if(execludedPaths is null)
                    {
                        execludedPaths = new[] { "/live", "/ready", "/metrics", "/metrics-text" };
                    }

                    var requestPath = ctx.Request.Path.Value;

                    if (ex is not null || ctx.Response.StatusCode > 499) // exception exist or response is 5xx indicating an error
                    {
                        return LogEventLevel.Error;
                    }
                    else if (execludedPaths.Any(x => requestPath!.StartsWith(x, StringComparison.OrdinalIgnoreCase))) // path is execluded
                    {
                        return LogEventLevel.Verbose;
                    }

                    return LogEventLevel.Information;
                };

                // Attach additional properties to the request completion event
                options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
                {
                    const string requestIdHeader = "Request-Id";

                    if (httpContext.Request.Headers.ContainsKey(requestIdHeader))
                    {
                        diagnosticContext.Set("CorrelationId", httpContext.Request.Headers[requestIdHeader].First());
                    }

                    // Attach additional properties to the request completion event
                    bool isAuthenticated = httpContext.User!.Identity!.IsAuthenticated;
                    string userId = "UnAuthenticated";
                    string username = "UnAuthenticated";
                    if (isAuthenticated)
                    {
                        userId = "User id not found";
                        username = "Username not found";
                    }


                    if (httpContext.User.HasClaim(x => x.Type == "sub"))
                    {
                        userId = httpContext.User.FindFirst("sub")!.Value;
                    }

                    if (httpContext.User.HasClaim(x => x.Type == "email"))
                    {
                        username = httpContext.User.FindFirst("email")!.Value;
                    }

                    diagnosticContext.Set("UserId", userId);
                    diagnosticContext.Set("Username", username);
                    diagnosticContext.Set("RequestHost", httpContext.Request.Host.Value);
                    diagnosticContext.Set("RequestScheme", httpContext.Request.Scheme);
                };
            });

            return app;
        }
    }
}
