using CA.Common.Logging;
using Serilog;

Log.Logger = LoggingHelper.CASerilogConfiguration("Identity").CreateLogger();

try
{
    Log.Information("Starting up");
    var builder = WebApplication.CreateBuilder(args);
    builder.Host.UseSerilog();

    // Add services to the container.
    builder.Services.AddControllersWithViews();

    var app = builder.Build();

    // Configure the HTTP request pipeline.

    app
        .UseHttpsRedirection()
        .UseStaticFiles()
        .UseCASerilog()
        .UseRouting();

    app.MapControllerRoute(
    name: "default",
    pattern: "{controller}/{action=Index}/{id?}");

    app.MapFallbackToFile("index.html");

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
