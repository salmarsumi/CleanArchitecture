﻿using CA.BFF.Endpoints;
using CA.Common;
using CA.Common.Logging;
using CA.Common.Middleware;
using CA.WebAngular.Endpoints;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Prometheus;
using Serilog;
using System.IdentityModel.Tokens.Jwt;
using Yarp.ReverseProxy.Transforms;

namespace CA.WebAngular
{
    public static class HostingExtensions
    {
        private const string AUTHORIZATION_HEADR = "Authorization";
        private const string ANTIFORGERY_COOKIE_NAME = "__Host-BFF-Antiforgery";

        public static WebApplicationBuilder ConfigureBuilder(this WebApplicationBuilder builder)
        {
            builder.Host.UseSerilog(LoggingHelper.CASerilogConfiguration("BFF"));

            // YARP
            builder.Services.AddReverseProxy()
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

            // Antiforgery
            builder.Services.AddAntiforgery(options =>
             {
                 options.Cookie.HttpOnly = true;
                 options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                 options.Cookie.Name = ANTIFORGERY_COOKIE_NAME;
                 options.HeaderName = Constants.CSRF_HEADER;
                 options.FormFieldName = Constants.CSRF_FORM_FIELD;
             });

            // Authentication
            builder.ConfigureAuthentication();

            // Authorization
            builder.ConfigureAuthorization();

            // Health Checks
            builder.ConfigureHealthChecks();

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
                .UseCASerilog()
                .UseExceptionHandler(ExceptionHandler.Handler)
                .UseHttpMetrics(options => options.ReduceStatusCodeCardinality())
                .UseStaticFiles()
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

                    var path = context.Request.Path.Value;

                    // Antiforgery
                    // Generate token on the first request.
                    // When using the Angular dev server as a proxy the postlogin is needed to set the token.
                    var antiforgery = app.Services.GetRequiredService<IAntiforgery>();
                    if (
                        path.Equals("/", StringComparison.OrdinalIgnoreCase) ||
                        path.Equals("/index.html", StringComparison.OrdinalIgnoreCase) ||
                        path.Equals("/account/postlogin", StringComparison.OrdinalIgnoreCase))
                    {
                        // The request token can be sent as a JavaScript-readable cookie, 
                        // and Angular uses the token as a header in every request.
                        var tokens = antiforgery.GetAndStoreTokens(context);
                        context.Response.Cookies.Append(Constants.CSRF_COOKIE_NAME, tokens.RequestToken,
                            new CookieOptions()
                            {
                                HttpOnly = false,
                                SameSite = SameSiteMode.Strict,
                                Secure = true
                            });
                    }
                    
                    // Force authentication
                    if (context.User.Identity.IsAuthenticated)
                    {
                        // Validate Antigorgery token
                        if (path.StartsWith("/api", StringComparison.OrdinalIgnoreCase) ||
                            path.StartsWith("/account/session", StringComparison.OrdinalIgnoreCase) ||
                            path.StartsWith("/account/logout", StringComparison.OrdinalIgnoreCase))
                        {
                            await antiforgery.ValidateRequestAsync(context);
                        }

                        await next();
                    }
                    // no need to authenticate health check and metrics endpoints
                    else if (path.Equals("/live", StringComparison.OrdinalIgnoreCase) ||
                             path.Equals("/ready", StringComparison.OrdinalIgnoreCase) ||
                             path.Equals("/metrics", StringComparison.OrdinalIgnoreCase))
                    {
                        await next();
                    }
                    else
                    {
                        await context.ChallengeAsync(OpenIdConnectDefaults.AuthenticationScheme);
                    }

                });

            // Endpoints
            app
                .MapAccountEndpoints()
                .MapHealthCheckEndpoits()
                .MapMetrics();
            
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
                    options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
                    options.SlidingExpiration = false;
                    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                })
                .AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, options =>
                {
                    options.Authority = builder.Configuration["TokenAuthority"];
                    
                    options.ClientId = "angular";
                    options.ClientSecret = "secret";
                    options.ResponseType = "code";

                    options.Scope.Add("openid");
                    options.Scope.Add("email");
                    options.Scope.Add("profile");
                    options.Scope.Add("api.full");
                    options.Scope.Add("authz.full");
                    options.Scope.Add("audit.full");

                    options.RequireHttpsMetadata = false;

                    options.UsePkce = true;
                    options.GetClaimsFromUserInfoEndpoint = true;
                    options.SaveTokens = true;

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
    }
}
