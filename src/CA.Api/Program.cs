using CA.Api;
using CA.Common.Logging;
using Serilog;

Log.Logger = LoggingHelper.CASerilogConfiguration("Api").CreateLogger();

try
{
    Log.Information("Starting up Api");
    var builder = WebApplication.CreateBuilder(args);

    // Configure Services
    builder.ConfigureBuilder();

    var app = builder.Build();
    
    // Migrate the databse
    // for production this should be optional and only triggered on command arguments
    // the migration should not happen on every run
    // app.MigrateDbContext<ApiDbContext>((_, __) => { });

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