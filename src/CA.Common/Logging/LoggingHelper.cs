using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.Grafana.Loki;

namespace CA.Common.Logging
{
    public static class LoggingHelper
    {
        public static ILogger CASerilogBootstrapConfiguration(string appName)
        {
            var config = new LoggerConfiguration();

            config
                .MinimumLevel.Information()
                .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
                .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Warning);

            config.Enrich.WithProperty("ApplicationContext", appName)
                .Enrich.FromLogContext()
                .WriteTo.Debug()
                .WriteTo.Console(outputTemplate:
                    "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:w4}]: {Message:lj} {CorrelationId}{NewLine}{Exception}");

            return config.CreateBootstrapLogger();
        }

        public static Action<HostBuilderContext, IServiceProvider, LoggerConfiguration> CASerilogConfiguration(string appName)
        {
            Action<HostBuilderContext, IServiceProvider, LoggerConfiguration> action = (context, services, configuration) =>
            {
                // check if detailed logging enabled
                if (string.Equals(context.Configuration["Serilog:DetailedLog"], bool.TrueString, StringComparison.OrdinalIgnoreCase))
                {
                    configuration.MinimumLevel.Verbose();
                }
                else
                {
                    configuration
                        .MinimumLevel.Information()
                        .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
                        .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Warning);
                }

                configuration.Enrich.WithProperty("ApplicationContext", appName)
                    .Enrich.FromLogContext()
                    .WriteTo.Debug()
                    .WriteTo.Console(outputTemplate:
                        "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:w4}]: {Message:lj} {CorrelationId}{NewLine}{Exception}");

                // check if Loki logging is enabled
                if (string.Equals(context.Configuration["Serilog:Loki:Enable"], bool.TrueString, StringComparison.OrdinalIgnoreCase) &&
                    !string.IsNullOrWhiteSpace(context.Configuration["Serilog:Loki:Address"]))
                {
                    configuration.WriteTo.GrafanaLoki(
                        context.Configuration["Serilog:Loki:Address"],
                        filtrationMode: LokiLabelFiltrationMode.Include,
                        filtrationLabels: new[] { "ApplicationContext", "level" },
                        createLevelLabel: true
                        //textFormatter: new LokiWithLevelJsonTextFormatter()
                        );
                }
            };

            return action;
        }
    }
}
