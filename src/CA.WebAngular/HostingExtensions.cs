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
        private const string TOKEN_NAME = "access_token";
        private const string AUTHORIZATION_HEADR = "Authorization";
        private const string CORRELATION_HEADER = "Request-Id";

        public static WebApplicationBuilder ConfigureBuilder(this WebApplicationBuilder builder)
        {
            builder.Host.UseSerilog();

            // YARP
            builder.Services.AddReverseProxy()
                .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"))
                .AddTransforms(builderContext =>
                {
                    builderContext.AddRequestTransform(async transforContext =>
                    {
                        // Correlation header
                        if (transforContext.HttpContext.Request.Headers.ContainsKey(CORRELATION_HEADER))
                        {
                            transforContext.ProxyRequest.Headers.Add(CORRELATION_HEADER, transforContext.HttpContext.Request.Headers[CORRELATION_HEADER][0]);
                        }

                        // Access token
                        var token = await transforContext.HttpContext.GetTokenAsync(TOKEN_NAME);
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
                })
                .AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, options =>
                {
                    options.Authority = "https://localhost:7127";

                    options.ClientId = "angular";
                    options.ClientSecret = "secret";
                    options.ResponseType = "code";

                    options.Scope.Clear();
                    options.Scope.Add("openid");
                    options.Scope.Add("profile");
                    options.Scope.Add("api");
                    options.Scope.Add("authorization");

                    options.UsePkce = true;
                    options.GetClaimsFromUserInfoEndpoint = true;
                    options.SaveTokens = true;

                    options.TokenValidationParameters.NameClaimType = "name";
                    options.TokenValidationParameters.RoleClaimType = "role";
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
