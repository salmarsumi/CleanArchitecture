using CA.Api.Domain.Entities;
using CA.Api.Domain.Interfaces;
using CA.Api.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CA.Api.Infrastructure.Repositories
{
    public class WeatherForecastRepository : IWeatherForecastRepository
    {
        private readonly IApiDbContext _context;

        public WeatherForecastRepository(IApiDbContext context)
        {
            _context = context;
        }

        public void Add(WeatherForecast entity)
        {
            _context.WeatherForcasts.Add(entity);
        }

        public void Remove(WeatherForecast entity)
        {
            _context.WeatherForcasts.Remove(entity);
        }

        public async Task<WeatherForecast> GetAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _context.WeatherForcasts
                .AsNoTracking()
                .Where(x => x.Id == id)
                .SingleOrDefaultAsync(cancellationToken);
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
