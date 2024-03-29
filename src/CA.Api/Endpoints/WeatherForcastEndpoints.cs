﻿using CA.Api.Application.WeatherForcast.Commands.Create;
using CA.Api.Application.WeatherForcast.Commands.Delete;
using CA.Api.Application.WeatherForcast.Queries;
using CA.Common.Authorization;
using CA.MediatR;
using MediatR;
using Microsoft.AspNetCore.Authorization;

namespace CA.Api.Endpoints
{
    public static class WeatherForcastEndpoints
    {
        public static WebApplication MapWeatherForcastEndpoints(this WebApplication app)
        {
            // GET /weather
            app.MapGet("/weather", [Authorize(nameof(AppPermissions.ViewWeather))] async (ISender mediator) =>
            {
                RequestResult<IEnumerable<WeatherForecastDto>> result = await mediator.Send(new GetAllWeatherForcastQuery());

                if (result.Success)
                {
                    return Results.Ok(result.Result);
                }

                return result.AsApiResult();
            }).RequireAuthorization();

            // POST /weather
            app.MapPost("/weather", [Authorize(nameof(AppPermissions.CreateWeather))] async (ISender mediator) =>
            {
                var result = await mediator.Send(new CreateWeatherForcastCommand
                {
                    Date = DateTime.Now.AddDays(Random.Shared.Next(1, 20)),
                    TemperatureC = 50,
                    Summary = "Hot"
                });

                if (result.Success)
                {
                    return Results.Created("/", result.Result);
                }

                return result.AsApiResult();

            }).RequireAuthorization();

            // DELETE /weather/id
            app.MapDelete("/weather/{id}", [Authorize(nameof(AppPermissions.DeleteWeather))] async (int id, ISender mediator) =>
            {
                RequestResult<Unit> result = await mediator.Send(new DeleteWeatherForcastCommand { Id = id });

                if (result.Success)
                {
                    return Results.NoContent();
                }

                return result.AsApiResult();
            }).RequireAuthorization();

            return app;
        }
    }
}
