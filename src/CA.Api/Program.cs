using CA.Api;
using CA.Common.Logging;
using CA.Common.Middleware;
using Serilog;

Log.Logger = LoggingHelper.CASerilogConfiguration("Api").CreateLogger();

try
{
    Log.Information("Starting up Api");
    var builder = WebApplication.CreateBuilder(args);

    // Configure Services
    builder.ConfigureBuilder();

    var app = builder.Build();
    app.UseExceptionHandler(ExceptionHandler.Handler);

    // Configure the HTTP request pipeline.
    app.ConfigurePipeline();

    app.Run();
}
catch(Exception ex)
{
    Log.Fatal(ex, "Host terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}