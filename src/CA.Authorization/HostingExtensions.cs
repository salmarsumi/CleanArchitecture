using CA.Authorization.Endpoints;
using CA.Common.Authorization.AspNetCore;
using CA.Common.Logging;
using CA.Common.Middleware;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Prometheus;
using Serilog;
using System.IdentityModel.Tokens.Jwt;

namespace CA.Authorization
{
    public static class HostingExtensions
    {
        public static WebApplicationBuilder ConfigureBuilder(this WebApplicationBuilder builder)
        {
            builder.Host.UseSerilog(LoggingHelper.CASerilogConfiguration("Authorization"));

            // Permissions
            builder.Services.AddLocalPolicyServices();

            builder
                // Authentication
                .ConfigureAuthentication()
                // Authorization
                .ConfigureAuthorization()
                // App Services
                .ConfigureAppServices()
                // Health Checks
                .ConfigureHealthChecks();

            return builder;
        }

        public static WebApplication ConfigurePipeline(this WebApplication app)
        {
            app
                .UseCASerilog()
                .UseExceptionHandler(ExceptionHandler.Handler)
                .UseHttpMetrics(options => options.ReduceStatusCodeCardinality())
                .UseAuthentication()
                .UseAuthorization();

            // Endpoints
            app
                .MapPolicyEndpoints()
                .MapHealthCheckEndpoits()
                .MapMetrics();

            return app;
        }

        public static WebApplicationBuilder ConfigureAuthentication(this WebApplicationBuilder builder)
        {
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear(); // prevent from mapping "sub" claim to nameidentifier.
            builder.Services
                .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
                {
                    options.Authority = builder.Configuration["TokenAuthority"];
                    options.TokenValidationParameters.ValidTypes = new[] { "at+jwt" };
                    options.TokenValidationParameters.ValidateAudience = true;
                    options.RequireHttpsMetadata = false;
                    options.Audience = "authz";

                    options.TokenValidationParameters.NameClaimType = "name";
                });

            return builder;
        }

        public static WebApplicationBuilder ConfigureHealthChecks(this WebApplicationBuilder builder)
        {
            builder.Services
                .AddHealthChecks()
                .AddCheck("self", () => HealthCheckResult.Healthy())
                // ready checks should use actual checks of external dependancies.
                .AddCheck("ready", () => HealthCheckResult.Healthy())
                .ForwardToPrometheus();

            return builder;
        }

        public static WebApplicationBuilder ConfigureAppServices(this WebApplicationBuilder builder)
        {
            // HttpContext
            builder.Services.AddHttpContextAccessor();

            // Caching
            builder.Services.AddDistributedMemoryCache();

            return builder;
        }

        public static WebApplicationBuilder ConfigureAuthorization(this WebApplicationBuilder builder)
        {
            builder.Services
                .AddAuthorization(options =>
                {
                    options.DefaultPolicy = new AuthorizationPolicyBuilder()
                        .RequireAuthenticatedUser()
                        .Build();
                });

            return builder;
        }
    }
}
