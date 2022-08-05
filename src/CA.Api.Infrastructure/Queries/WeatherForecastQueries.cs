using CA.Api.Application.WeatherForcast.Queries;
using CA.Api.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CA.Api.Infrastructure.Queries
{
    public class WeatherForecastQueries : IWeatherForecastQueries
    {
        private readonly IApiDbContext _context;

        public WeatherForecastQueries(IApiDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<WeatherForecastDto>> GetAllWeatherForecastAsync(CancellationToken cancellationToken = default)
        {
            return
                await _context.WeatherForcasts.AsNoTracking().Select(e => new WeatherForecastDto
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
