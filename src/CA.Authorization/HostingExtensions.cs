using CA.Authorization.Endpoints;
using CA.Common.Authorization.AspNetCore;
using CA.Common.Logging;
using CA.Common.Middleware;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Serilog;
using System.IdentityModel.Tokens.Jwt;

namespace CA.Authorization
{
    public static class HostingExtensions
    {
        public static WebApplicationBuilder ConfigureBuilder(this WebApplicationBuilder builder)
        {
            builder.Host.UseSerilog();

            // Permissions
            builder.AddLocalPolicyServices();

            builder
                .ConfigureAuthentication() // Authentication
                .ConfigureAuthorization() // Authorization
                .ConfigureAppServices(); // App Services

            return builder;
        }

        public static WebApplication ConfigurePipeline(this WebApplication app)
        {
            app
                .UseCASerilog()
                .UseExceptionHandler(ExceptionHandler.Handler)
                .UseAuthentication()
                .UseAuthorization();

            app.MapPolicyEndpoints();

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
                });

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
