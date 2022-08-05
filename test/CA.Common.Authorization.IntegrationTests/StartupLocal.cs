using CA.Common.Authorization.AspNetCore;
using CA.Common.Authorization.IntegrationTests.Controllers;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using System.Reflection;

namespace CA.Common.Authorization.IntegrationTests
{
    public class StartupLocal
    {
        public StartupLocal(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(x => x.Events = new CookieAuthenticationEvents
                {
                    OnRedirectToAccessDenied = context =>
                    {
                        context.Response.StatusCode = 403;
                        return Task.CompletedTask;
                    },
                    OnRedirectToLogin = context =>
                    {
                        context.Response.StatusCode = 401;
                        return Task.CompletedTask;
                    }
                });
            services.AddAuthorization();
            services.AddDistributedMemoryCache();

            services.AddLocalPolicyServices()
                .AddAuthorizationPermissionPolicies();

            services.AddMvc();
            services.AddControllers()
            .ConfigureApplicationPartManager(apm =>
                apm.FeatureProviders.Add(new TestControllerFeatureProvider()));
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseRouting();
            app.UseMiddleware<AutoAuthorizeMiddleware>();
            app.UseAuthorization();
            app.UseEndpoints(endpoints => endpoints.MapControllers());
        }
    }

    public class TestControllerFeatureProvider : IApplicationFeatureProvider<ControllerFeature>
    {
        public void PopulateFeature(IEnumerable<ApplicationPart> parts, ControllerFeature feature)
        {
            feature.Controllers.Remove(typeof(RemoteController).GetTypeInfo());
        }
    }
}
