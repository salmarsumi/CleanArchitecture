using CA.Api.Domain.Entities;

namespace CA.Api.Domain.Interfaces
{
    public interface IWeatherForecastRepository : IRepository<WeatherForecast, int>
    {
    }
}
