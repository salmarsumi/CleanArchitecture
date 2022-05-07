using CA.Api.Endpoints;
using CA.Common.Logging;
using Microsoft.AspNetCore.Authorization;
using Serilog;

namespace CA.Api
{
    public static class HostingExtensions
    {
        public static WebApplicationBuilder ConfigureBuilder(this WebApplicationBuilder builder)
        {
            builder.Host.UseSerilog();

            return builder; 
        }

        public static WebApplication ConfigurePipeline(this WebApplication app)
        {
            app
                .UseHttpsRedirection()
                .UseCASerilog();

            app.MapWeatherForcastEndpoints();

            return app;
        }

        public static WebApplicationBuilder ConfigureAuthentication(this WebApplicationBuilder builder)
        {

            return builder;
        }

        public static IServiceCollection ConfigureAuthorization(this IServiceCollection services)
        {
            services
                .AddAuthorization(options =>
                {
                    options.DefaultPolicy = new AuthorizationPolicyBuilder()
                        .RequireAuthenticatedUser()
                        .Build();
                });

            return services;
        }
    }
}
