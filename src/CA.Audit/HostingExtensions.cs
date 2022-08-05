using CA.Audit.Infrastructure;
using CA.Common.HttpMessageHandlers;
using CA.Common.Logging;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Http;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System.IdentityModel.Tokens.Jwt;
using CA.Common.Authorization.AspNetCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Prometheus;
using MassTransit;
using CA.Common.Services;
using CA.Audit.Consumers;
using CA.Common.Middleware;
using CA.Audit.Endpoints;
using CA.MediatR;
using CA.Audit.Application.Audit;
using CA.Audit.Application.Access;

namespace CA.Audit
{
    public static class HostingExtensions
    {
        public static WebApplicationBuilder ConfigureBuilder(this WebApplicationBuilder builder)
        {
            builder.Host.UseSerilog(LoggingHelper.CASerilogConfiguration("Audit"));

            // MediatR
            builder.Services.AddMediatRServices<GetAllAuditQuery>();
            // DbContext
            builder.ConfigureDbContext();
            // App Services
            builder.ConfigureAppServices();
            // Authentication
            builder.ConfigureAuthentication();
            // Authorization
            builder.ConfigureAuthorization();
            // Healthchecks
            builder.ConfigureHealthChecks();
            // MassTransit
            builder.ConfigureMassTransit();

            // Permissions
            builder.Services
                .AddRemotePolicyServices()
                .AddAuthorizationPermissionPolicies()
                .AddRemotePolicyHttpClient(builder.Configuration["AuthorizationService"]);

            // HttpClient logging
            builder.Services.Replace(ServiceDescriptor.Singleton<IHttpMessageHandlerBuilderFilter, CAHttpMessageHandlerBuilderFilter>());

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
                .MapAuditEndpoints()
                .MapAccessEndpoints()
                .MapHealthCheckEndpoits()
                .MapMetrics();

            return app;
        }

        public static WebApplicationBuilder ConfigureAuthentication(this WebApplicationBuilder builder)
        {
            // prevent from mapping "sub" claim to nameidentifier.
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

            builder.Services
                .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
                {
                    options.Authority = builder.Configuration["TokenAuthority"];
                    options.TokenValidationParameters.ValidTypes = new[] { "at+jwt" };
                    options.TokenValidationParameters.ValidateAudience = true;
                    options.RequireHttpsMetadata = false;
                    options.Audience = "audit";

                    options.TokenValidationParameters.NameClaimType = "name";
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
            builder.Services.AddDbContext<AuditDbContext>(options =>
            {
                options
                    .UseInMemoryDatabase("AuditDb")
                    .ConfigureWarnings(b => b.Ignore(InMemoryEventId.TransactionIgnoredWarning));
            });

            builder.Services.AddScoped<IAuditDbContext, AuditDbContext>();

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

        public static WebApplicationBuilder ConfigureMassTransit(this WebApplicationBuilder builder)
        {
            if (string.Equals(builder.Configuration["MassTransit:Enable"], bool.TrueString, StringComparison.OrdinalIgnoreCase))
            {
                builder.Services.AddMassTransit(config =>
                {
                    config.SetKebabCaseEndpointNameFormatter();

                    config.AddConsumer<AuditEntryConsumer>(cfg =>
                    {
                        cfg.UseMessageRetry(x =>
                        {
                            x.Exponential(5, TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(30), TimeSpan.FromSeconds(5));
                        });
                    });

                    config.AddConsumer<AccessEntryConsumer>(cfg =>
                    {
                        cfg.UseMessageRetry(x =>
                        {
                            x.Exponential(5, TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(30), TimeSpan.FromSeconds(5));
                        });
                    });

                    config.UsingRabbitMq((context, cfg) =>
                    {
                        cfg.Host(builder.Configuration["MassTransit:Host"], builder.Configuration["MassTransit:VHost"], config =>
                        {
                            config.Username(builder.Configuration["MassTransit:Username"]);
                            config.Password(builder.Configuration["MassTransit:Password"]);
                            config.PublisherConfirmation = true;
                        });
                        cfg.Durable = true;
                        cfg.ConfigureEndpoints(context);
                    });
                });
            }

            return builder;
        }

        public static WebApplicationBuilder ConfigureAppServices(this WebApplicationBuilder builder)
        {
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddScoped<ICurrentRequestService, CurrentRequestService>();
            builder.Services.AddDistributedMemoryCache();

            // Queries
            builder.Services.AddScoped<IAuditQueries, AuditQueries>();
            builder.Services.AddScoped<IAccessQueries, AccessQueries>();

            return builder;
        }
    }
}
