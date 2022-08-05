using CA.Api.Domain.Entities;
using CA.Common.EF;
using CA.Common.SeedWork;
using CA.Common.Services;
using Microsoft.EntityFrameworkCore;

namespace CA.Api.Infrastructure.Data
{
    public class ApiDbContext : BaseTransactionalDbContext<ApiDbContext>, IApiDbContext
    {
        public ApiDbContext(DbContextOptions<ApiDbContext> options, ICurrentRequestService currentUserService, IDomainEventService domainEventService) 
            : base(options, currentUserService, domainEventService)
        { }

        public DbSet<WeatherForecast> WeatherForcasts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            new WeatherForcastEntityTypeConfiguration().Configure(modelBuilder.Entity<WeatherForecast>());
            base.OnModelCreating(modelBuilder);
        }
    }
}
