using CA.Authorization;
using CA.Common.Logging;
using CA.Common.Middleware;
using Serilog;

Log.Logger = LoggingHelper.CASerilogConfiguration("Authorization").CreateLogger();

try
{
    Log.Information("Starting up Authorization");
    var builder = WebApplication.CreateBuilder(args);

    // Configure Services
    builder.ConfigureBuilder();

    var app = builder.Build();
    // Configure the HTTP request pipeline.
    app.ConfigurePipeline();

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Host terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}