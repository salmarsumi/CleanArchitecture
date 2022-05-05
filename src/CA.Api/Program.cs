using CA.Common.Logging;
using Serilog;

Log.Logger = LoggingHelper.CASerilogConfiguration("Api").CreateLogger();

try
{
    Log.Information("Starting up Api");
    var builder = WebApplication.CreateBuilder(args);
    builder.Host.UseSerilog();

    // Add services to the container.

    var app = builder.Build();

    // Configure the HTTP request pipeline.

    app
        .UseHttpsRedirection()
        .UseCASerilog();

    app.MapGet("/", () => Results.Ok(new[] { new { id = 1, name = "Name 1" }, new { id = 2, name = "Name 2" } }));

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