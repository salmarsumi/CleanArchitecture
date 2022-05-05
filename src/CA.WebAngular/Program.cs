using CA.Common.Logging;
using CA.WebAngular;
using Serilog;

Log.Logger = LoggingHelper.CASerilogConfiguration("Identity").CreateLogger();

try
{
    Log.Information("Starting up WebAngular");
    var builder = WebApplication.CreateBuilder(args);
    builder.Host.UseSerilog();

    // Add services to the container.
    builder.ConfigureServices();

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
