using CA.Common.Logging;
using CA.Identity;
using Serilog;

Log.Logger = LoggingHelper.CASerilogBootstrapConfiguration("Identity");

try
{
    Log.Information("Starting up Identity");
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