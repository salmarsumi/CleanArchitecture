using CA.Audit;
using CA.Common.Logging;
using Serilog;

Log.Logger = LoggingHelper.CASerilogBootstrapConfiguration("Audit");

try
{
    Log.Information("Starting up Audit");
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