using CA.Common.Authorization.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using System.Net;
using System.Net.Http.Headers;

namespace CA.Common.Authorization.AspNetCore
{
    /// <summary>
    /// Helper object to build the PolicyServer DI configuration
    /// </summary>
    public class ServerBuilder
    {
        public IServiceCollection Services { get; }

        public ServerBuilder(IServiceCollection services)
        {
            Services = services;
        }

        public ServerBuilder AddAuthorizationPermissionPolicies()
        {
            //Services.AddAuthorization();
            Services.AddTransient<IAuthorizationPolicyProvider, PolicyAuthorizationProvider>();
            Services.AddTransient<IAuthorizationHandler, PermissionHandler>();

            return this;
        }

        public ServerBuilder AddRemotePolicyHttpClient(string authServiceBaseUrl)
        {
            // configure the HttpClient instance that will be injected into the RemotePolicyOperations constructor
            Services.AddHttpClient(Constants.AUTHORIZATION_API_HTTP_CLIENT_NAME, async (p, c) =>
            {
                var httpContextAccessor = p.GetRequiredService<IHttpContextAccessor>();

                if (httpContextAccessor.HttpContext.Request.Headers.ContainsKey(Common.Constants.CORRELATION_HEADER))
                {
                    c.DefaultRequestHeaders.Add(Common.Constants.CORRELATION_HEADER,
                        httpContextAccessor.HttpContext.Request.Headers[Common.Constants.CORRELATION_HEADER].ToString());
                }

                var accessToken = await httpContextAccessor.HttpContext
                    .GetTokenAsync(Constants.AUTHENTICATION_SCHEME, Common.Constants.TOKEN_NAME);

                if (!string.IsNullOrEmpty(accessToken))
                {
                    c.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(Constants.AUTHENTICATION_SCHEME, accessToken);
                }

                c.BaseAddress = new Uri(authServiceBaseUrl);
                c.DefaultRequestHeaders.Add("Accept", "application/json");
            })
            // set the life time of the message handler to 5 minutes up from the default 3
            .SetHandlerLifetime(TimeSpan.FromMinutes(5))
            // retry the request on transient errors up to 6 times with increased wait between attempts
            .AddTransientHttpErrorPolicy(p => p
                .OrResult(msg => msg.StatusCode == HttpStatusCode.NotFound)
                .WaitAndRetryAsync(6, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))))
            // circuit break after 5 attempts for 30 seconds
            .AddTransientHttpErrorPolicy(p => p
                .CircuitBreakerAsync(5, TimeSpan.FromSeconds(30)));

            return this;
        }
    }

    /// <summary>
    /// Helper class to configure DI
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        public static ServerBuilder AddLocalPolicyServices(this IServiceCollection services)
        {
            services.AddScoped<IPolicyOperations, PolicyOperations>();

            return new ServerBuilder(services);
        }

        public static ServerBuilder AddRemotePolicyServices(this IServiceCollection services)
        {
            services.AddScoped<IPolicyOperations, RemotePolicyOperations>();

            return new ServerBuilder(services);
        }
    }
}
