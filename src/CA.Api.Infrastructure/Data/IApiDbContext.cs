using Entities = CA.Api.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CA.Api.Infrastructure.Data
{
    public interface IApiDbContext
    {
        DbSet<Entities.WeatherForecast> WeatherForcasts { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}
