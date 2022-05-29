using CA.Common;
using CA.Common.Logging;
using CA.WebAngular.Endpoints;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Serilog;
using System.IdentityModel.Tokens.Jwt;
using Yarp.ReverseProxy.Transforms;

namespace CA.WebAngular
{
    public static class HostingExtensions
    {
        private const string AUTHORIZATION_HEADR = "Authorization";

        public static WebApplicationBuilder ConfigureBuilder(this WebApplicationBuilder builder)
        {
            builder.Host.UseSerilog();

            // YARP
            builder.Services.AddReverseProxy()
                //.ConfigureHttpClient((_, handler) => handler.ActivityHeadersPropagator = null)
                .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"))
                .AddTransforms(builderContext =>
                {
                    builderContext.AddRequestTransform(async transforContext =>
                    {
                        // Access token
                        var token = await transforContext.HttpContext.GetTokenAsync(Constants.TOKEN_NAME);
                        if (token is not null)
                        {
                            transforContext.ProxyRequest.Headers.Add(AUTHORIZATION_HEADR, $"Bearer {token}");
                        }
                    });
                });

            // Authentication
            builder.ConfigureAuthentication();

            // Authorization
            builder.Services.ConfigureAuthorization();

            return builder;
        }

        public static WebApplication ConfigurePipeline(this WebApplication app)
        {
            app
                .UseHttpsRedirection()
                .UseStaticFiles()
                .UseCASerilog()
                .UseRouting();

            app
                .UseAuthentication()
                .UseAuthorization()
                .Use(async (context, next) =>
                {
                    // write the correlation id into the response if its found
                    if (!context.Response.Headers.ContainsKey(Constants.CORRELATION_HEADER))
                    {
                        context.Response.Headers.Add(Constants.CORRELATION_HEADER, context.Request.Headers[Constants.CORRELATION_HEADER].First());
                    }

                    if (context.User.Identity.IsAuthenticated)
                    {
                        await next();
                    }
                    else
                    {
                        await context.ChallengeAsync(OpenIdConnectDefaults.AuthenticationScheme);
                    }

                });

            app.MapAccountEndpoints();
            app.MapFallbackToFile("index.html").RequireAuthorization();
            app.MapReverseProxy();

            return app;
        }

        public static WebApplicationBuilder ConfigureAuthentication(this WebApplicationBuilder builder)
        {
            JwtSecurityTokenHandler.DefaultMapInboundClaims = false;

            builder.Services
                .AddAuthentication(options =>
                {
                    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
                    options.DefaultSignOutScheme = OpenIdConnectDefaults.AuthenticationScheme;
                })
                .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
                {
                    options.Cookie.Name = "__Host-CA-bff";
                    options.Cookie.SameSite = SameSiteMode.Strict;
                    options.ExpireTimeSpan = TimeSpan.FromHours(1);
                    options.SlidingExpiration = false;
                })
                .AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, options =>
                {
                    options.Authority = "https://localhost:7127";

                    options.ClientId = "angular";
                    options.ClientSecret = "secret";
                    options.ResponseType = "code";

                    options.Scope.Add("openid");
                    options.Scope.Add("email");
                    options.Scope.Add("profile");
                    options.Scope.Add("api.full");
                    options.Scope.Add("authz.full");

                    options.RequireHttpsMetadata = false;

                    options.UsePkce = true;
                    options.GetClaimsFromUserInfoEndpoint = true;
                    options.SaveTokens = true;
                });

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
