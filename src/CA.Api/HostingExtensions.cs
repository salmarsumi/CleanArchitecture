using CA.Api.Application.Interfaces;
using CA.Api.Application.WeatherForcast.Commands.Create;
using CA.Api.Endpoints;
using CA.Api.Infrastructure.Data;
using CA.Common.EF;
using CA.Common.Logging;
using CA.Common.Middleware;
using CA.Common.Services;
using CA.MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Serilog;
using SMD.Security.Authorization.AspNetCore;
using System.IdentityModel.Tokens.Jwt;

namespace CA.Api
{
    public static class HostingExtensions
    {
        public static WebApplicationBuilder ConfigureBuilder(this WebApplicationBuilder builder)
        {
            builder.Host.UseSerilog();

            // MediatR
            builder.Services.AddMediatRServices<CreateWeatherForcastCommand>();
            // DbContext
            builder.ConfigureDbContext();
            // App Services
            builder.ConfigureAppServices();
            // Authentication
            builder.ConfigureAuthentication();
            // Authorization
            builder.ConfigureAuthorization();
            
            // Permissions
            builder.Services
                .AddRemotePolicyServices(builder.Configuration, "Policy")
                .AddAuthorizationPermissionPolicies()
                .AddRemotePolicyHttpClient();

            return builder; 
        }

        public static WebApplication ConfigurePipeline(this WebApplication app)
        {
            app.UseExceptionHandler(ExceptionHandler.Handler);

            app
                .UseHttpsRedirection()
                .UseCASerilog();

            app
                .UseAuthentication()
                .UseAuthorization();

            app.MapWeatherForcastEndpoints();

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
                    options.Audience = "api";
                });

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

        public static WebApplicationBuilder ConfigureDbContext(this WebApplicationBuilder builder)
        {
            // register the EF DbContext
            builder.Services.AddDbContext<ApiDbContext>(options =>
            {
                options
                    .UseInMemoryDatabase("ApiDb")
                    .ConfigureWarnings(b => b.Ignore(InMemoryEventId.TransactionIgnoredWarning));
            });

            builder.Services.AddScoped<IApiDbContext>(sp => sp.GetRequiredService<ApiDbContext>());
            builder.Services.AddScoped<ITransactionalDbContext>(sp => sp.GetRequiredService<ApiDbContext>());

            return builder;
        }

        public static WebApplicationBuilder ConfigureAppServices(this WebApplicationBuilder builder)
        {
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
            builder.Services.AddDistributedMemoryCache();

            return builder;
        }
    }
}
