﻿using Microsoft.AspNetCore.Diagnostics.HealthChecks;

namespace CA.Authorization.Endpoints
{
    public static class HealthCheckEndpoints
    {
        public static WebApplication MapHealthCheckEndpoits(this WebApplication app)
        {
            app
                .MapHealthChecks("/live", new HealthCheckOptions
                {
                    Predicate = r => r.Name.Contains("self")
                });

            app
                .MapHealthChecks("/ready", new HealthCheckOptions
                {
                    Predicate = r => r.Name.Contains("ready"),
                });

            return app;
        }
    }
}
