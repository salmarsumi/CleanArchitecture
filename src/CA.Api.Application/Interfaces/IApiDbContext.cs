using Entities = CA.Api.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CA.Api.Application.Interfaces
{
    public interface IApiDbContext
    {
        DbSet<Entities.WeatherForcast> WeatherForcasts { get; set; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}
