using CA.Api.Application.Interfaces;
using CA.Api.Domain.Entities;
using CA.Common.EF;
using CA.Common.Services;
using Microsoft.EntityFrameworkCore;

namespace CA.Api.Infrastructure.Data
{
    public class ApiDbContext : BaseTransactionalDbContext<ApiDbContext>, IApiDbContext
    {
        public ApiDbContext(DbContextOptions<ApiDbContext> options, ICurrentUserService currentUserService) : base(options, currentUserService)
        { }

        public DbSet<WeatherForcast> WeatherForcasts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            new WeatherForcastEntityTypeConfiguration().Configure(modelBuilder.Entity<WeatherForcast>());
            base.OnModelCreating(modelBuilder);
        }
    }
}
