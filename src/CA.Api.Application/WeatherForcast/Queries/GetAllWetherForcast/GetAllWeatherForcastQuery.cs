﻿using CA.Api.Application.Interfaces;
using CA.Common.Permissions;
using CA.MediatR;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CA.Api.Application.WeatherForcast.Queries
{
    public class GetAllWeatherForcastQuery : IRequest<IEnumerable<WeatherForecastDto>>, IAuthorizedRequest
    {
        public IEnumerable<string> GetRequiredPermissions()
        {
            return new[] { AppPermissions.ViewWeather };
        }
    }

    public class GetAllWeatherForcastQueryHandler : IRequestHandler<GetAllWeatherForcastQuery, IEnumerable<WeatherForecastDto>>
    {
        private readonly IApiDbContext _context;

        public GetAllWeatherForcastQueryHandler(IApiDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<WeatherForecastDto>> Handle(GetAllWeatherForcastQuery request, CancellationToken cancellationToken)
        {
            return await _context.WeatherForcasts.Select(e => new WeatherForecastDto
            {
                Id = e.Id,
                Date = e.Date,
                Summary = e.Summary,
                TemperatureC = e.TemperatureC
            })
                .ToListAsync(cancellationToken);
        }
    }
}