using CA.Api.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CA.Api.Infrastructure.Data
{
    public class WeatherForcastEntityTypeConfiguration : IEntityTypeConfiguration<WeatherForcast>
    {
        public void Configure(EntityTypeBuilder<WeatherForcast> builder)
        {
            builder.ToTable("Weather_Forcast");
            builder.HasKey(x => x.Id);
        }
    }
}
