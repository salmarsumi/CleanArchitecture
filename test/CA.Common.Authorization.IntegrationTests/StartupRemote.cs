using CA.Common.Authorization.AspNetCore;
using CA.Common.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CA.Common.Authorization.IntegrationTests
{
    public class StartupRemote
    {
        public StartupRemote(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
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
            services.AddControllers();
            services.AddHttpContextAccessor();
            services.AddScoped<ICurrentRequestService, CurrentRequestService>();
            services.AddRemotePolicyServices()
                .AddAuthorizationPermissionPolicies()
                .AddRemotePolicyHttpClient("https://localhost");
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseRouting();
            app.UseMiddleware<AutoAuthorizeMiddleware>();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints => endpoints.MapControllers());
        }
    }
}
