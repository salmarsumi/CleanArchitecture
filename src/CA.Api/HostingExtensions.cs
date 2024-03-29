﻿using CA.Api.Application.WeatherForcast.Commands.Create;
using CA.Api.Application.WeatherForcast.Queries;
using CA.Api.Domain.Interfaces;
using CA.Api.Endpoints;
using CA.Api.Infrastructure.Data;
using CA.Api.Infrastructure.Queries;
using CA.Api.Infrastructure.Repositories;
using CA.Common.Authorization.AspNetCore;
using CA.Common.EF;
using CA.Common.HttpMessageHandlers;
using CA.Common.Logging;
using CA.Common.Middleware;
using CA.Common.Services;
using CA.MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Http;
using Serilog;
using System.IdentityModel.Tokens.Jwt;
using Prometheus;
using MassTransit;

namespace CA.Api
{
    public static class HostingExtensions
    {
        public static WebApplicationBuilder ConfigureBuilder(this WebApplicationBuilder builder)
        {
            builder.Host.UseSerilog(LoggingHelper.CASerilogConfiguration("Api"));

            // MediatR
            builder.Services.AddMediatRServices<CreateWeatherForcastCommand>(isTransactional: true);
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
                .MapWeatherForcastEndpoints()
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
                    options.Audience = "api";

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
            builder.Services.AddDbContext<ApiDbContext>(options =>
            {
                options
                    .UseInMemoryDatabase("ApiDb")
                    .ConfigureWarnings(b => b.Ignore(InMemoryEventId.TransactionIgnoredWarning));
            });

            builder.Services.AddScoped<IApiDbContext>(sp => sp.GetRequiredService<ApiDbContext>());
            builder.Services.AddScoped<ITransactionalDbContext>(sp => sp.GetRequiredService<ApiDbContext>());

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

            // Repositories
            builder.Services.AddScoped<IWeatherForecastRepository, WeatherForecastRepository>();

            // Queries
            builder.Services.AddScoped<IWeatherForecastQueries, WeatherForecastQueries>();

            return builder;
        }
    }
}
