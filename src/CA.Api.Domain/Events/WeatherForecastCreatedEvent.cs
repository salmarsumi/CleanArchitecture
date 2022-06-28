using CA.Api.Domain.Entities;
using CA.Common.SeedWork;

namespace CA.Api.Domain.Events
{
    public class WeatherForecastCreatedEvent: DomainEvent
    {
        public WeatherForecastCreatedEvent(WeatherForecast item)
        {
            Item = item;
        }

        public WeatherForecast Item { get; }
    }
}
