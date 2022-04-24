using CA.Common.Logging;
using Serilog;

Log.Logger = LoggingHelper.CASerilogConfiguration("Api").CreateLogger();

try
{
    Log.Information("Starting up");
    var builder = WebApplication.CreateBuilder(args);
    builder.Host.UseSerilog();

    // Add services to the container.

    var app = builder.Build();

    // Configure the HTTP request pipeline.

    app
        .UseHttpsRedirection()
        .UseCASerilog();

    app.MapGet("/", () => "Home Page");

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