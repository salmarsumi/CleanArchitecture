namespace CA.Api.Endpoints
{
    public static class WeatherForcastEndpoints
    {
        public static WebApplication MapWeatherForcastEndpoints(this WebApplication app)
        {
            app.MapGet("/", () => Results.Ok(new[] { new { id = 1, name = "Name 1" }, new { id = 2, name = "Name 2" } }));

            return app;
        }
    }
}
