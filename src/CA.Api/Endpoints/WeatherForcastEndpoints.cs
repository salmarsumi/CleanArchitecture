using CA.Api.Application.WeatherForcast.Commands.Create;
using CA.Api.Application.WeatherForcast.Commands.Delete;
using CA.Api.Application.WeatherForcast.Queries;
using CA.Common.Permissions;
using MediatR;
using Microsoft.AspNetCore.Authorization;

namespace CA.Api.Endpoints
{
    public static class WeatherForcastEndpoints
    {
        public static WebApplication MapWeatherForcastEndpoints(this WebApplication app)
        {
            // GET /weather
            app.MapGet("/weather", [Authorize(nameof(AppPermissions.ViewWeather))] async (ISender mediator, HttpContext context) =>
            {
                var result = await mediator.Send(new GetAllWeatherForcastQuery());

                return Results.Ok(result);
            }).RequireAuthorization();

            // POST /weather
            app.MapPost("/weather", [Authorize(nameof(AppPermissions.CreateWeather))] async (ISender mediator) =>
            {
                var id = await mediator.Send(new CreateWeatherForcastCommand
                {
                    Date = DateTime.Now.AddDays(Random.Shared.Next(1, 20)),
                    TemperatureC = 50,
                    Summary = "Hot"
                });

                return Results.Created("/", id);
            }).RequireAuthorization();

            // DELETE /weather/id
            app.MapDelete("/weather/{id}", [Authorize(nameof(AppPermissions.DeleteWeather))] async (int id, ISender mediator) =>
            {
                await mediator.Send(new DeleteWeatherForcastCommand { Id = id });

                return Results.NoContent();
            }).RequireAuthorization();

            return app;
        }
    }
}
