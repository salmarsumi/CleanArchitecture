using CA.Api.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CA.Api.Application.WeatherForcast.Queries.GetAllWetherForcast
{
    public class GetAllWeatherForcastQuery : IRequest<IEnumerable<WeatherForecastDto>>
    {
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
                Date = e.Date,
                Summary = e.Summary,
                TemperatureC = e.TemperatureC
            })
                .ToListAsync(cancellationToken);
        }
    }
}
