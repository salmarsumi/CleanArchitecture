using CA.Common.Logging;
using CA.Common.Services;
using CA.Identity.Endpoints;
using CA.Identity.Services;
using IdentityServerHost;
using MassTransit;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Prometheus;
using Serilog;

namespace CA.Identity
{
    public static class HostingExtensions
    {
        public static WebApplicationBuilder ConfigureBuilder(this WebApplicationBuilder builder)
        {
            builder.Host.UseSerilog(LoggingHelper.CASerilogConfiguration("Identity"));

            builder.Services.AddRazorPages();

            builder.Services.AddIdentityServer(options =>
            {
                // Have to set issuer uri for docker environments.
                // This will allow the api application to connect
                // to the IdenetityServer from inside the docker
                // network without the need to make the call to the
                // host gateway.
                if (!string.IsNullOrWhiteSpace(builder.Configuration["ExternalAddress"]))
                {
                    options.IssuerUri = builder.Configuration["ExternalAddress"];
                }

                options.Authentication.CookieLifetime = TimeSpan.FromMinutes(60);
                options.Authentication.CookieSlidingExpiration = false;
                options.Authentication.CookieSameSiteMode = SameSiteMode.Strict;
            })
                .AddInMemoryClients(Config.Clients(builder.Configuration["ClientUri"]))
                .AddInMemoryIdentityResources(Config.Resources())
                .AddInMemoryApiResources(Config.Apis())
                .AddInMemoryApiScopes(Config.Scopes())
                .AddTestUsers(TestUsers.Users);

            builder.Services.AddAuthorization();

            // Health Checks
            builder.ConfigureHealthChecks();
            // MassTranssit
            builder.ConfigureMassTransit();

            builder.Services.AddScoped<IAccessEventPublisher, AccessEventPublisher>();
            builder.Services.AddScoped<ICurrentRequestService, CurrentRequestService>();

            return builder;
        }

        public static WebApplication ConfigurePipeline(this WebApplication app)
        {
            var forwardedHeadersOptions = new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            };
            forwardedHeadersOptions.KnownProxies.Clear();
            forwardedHeadersOptions.KnownNetworks.Clear();

            app
                .UseForwardedHeaders(forwardedHeadersOptions)
                .UseStaticFiles()
                .UseCASerilog()
                .UseRouting()
                .UseHttpMetrics(options => options.ReduceStatusCodeCardinality())
                .UseIdentityServer()
                .UseAuthorization();

            app.MapRazorPages().RequireAuthorization();

            // Endpoints
            app
                .MapHealthCheckEndpoits()
                .MapMetrics();

            return app;
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
    }
}
