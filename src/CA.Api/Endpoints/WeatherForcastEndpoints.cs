using CA.Api.Application.WeatherForcast.Commands.Create;
using CA.Api.Application.WeatherForcast.Commands.Delete;
using CA.Api.Application.WeatherForcast.Queries;
using MediatR;

namespace CA.Api.Endpoints
{
    public static class WeatherForcastEndpoints
    {
        public static WebApplication MapWeatherForcastEndpoints(this WebApplication app)
        {
            app.MapGet("/weather", async (ISender mediator) =>
            {
                var result = await mediator.Send(new GetAllWeatherForcastQuery());

                return Results.Ok(result);
            }).RequireAuthorization();

            app.MapPost("/weather", async (ISender mediator) =>
            {
                var id = await mediator.Send(new CreateWeatherForcastCommand
                {
                    Date = DateTime.Now.AddDays(Random.Shared.Next(1, 20)),
                    TemperatureC = 50,
                    Summary = "Hot"
                });

                return Results.Created("/", id);
            }).RequireAuthorization();

            app.MapDelete("/weather/{id}", async (int id, ISender mediator) =>
            {
                await mediator.Send(new DeleteWeatherForcastCommand { Id = id });

                return Results.NoContent();
            }).RequireAuthorization();

            return app;
        }
    }
}
