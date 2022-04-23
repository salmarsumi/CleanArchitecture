using Serilog;
using Serilog.Events;

namespace CA.Common.Logging
{
    public static class LoggingHelper
    {
        public static LoggerConfiguration CASerilogConfiguration(string appName)
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

            return config;
        }
    }
}
