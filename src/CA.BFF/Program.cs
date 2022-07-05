using CA.Common.Logging;
using CA.WebAngular;
using Serilog;

Log.Logger = LoggingHelper.CASerilogBootstrapConfiguration("BFF");

try
{
    Log.Information("Starting up WebAngular");
    var builder = WebApplication.CreateBuilder(args);

    // Add services to the container.
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
