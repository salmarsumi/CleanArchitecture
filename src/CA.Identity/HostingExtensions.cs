using CA.Common.Logging;
using IdentityServerHost;
using Microsoft.AspNetCore.HttpOverrides;
using Serilog;

namespace CA.Identity
{
    public static class HostingExtensions
    {
        public static WebApplicationBuilder ConfigureBuilder(this WebApplicationBuilder builder)
        {
            builder.Host.UseSerilog();

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
                .UseIdentityServer()
                .UseAuthorization();

            app.MapRazorPages().RequireAuthorization();

            return app;
        }
    }
}
