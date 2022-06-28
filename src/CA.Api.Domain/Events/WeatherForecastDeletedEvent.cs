using CA.Api.Domain.Entities;
using CA.Common.SeedWork;

namespace CA.Api.Domain.Events
{
    public class WeatherForecastDeletedEvent : DomainEvent
    {
        public WeatherForecastDeletedEvent(WeatherForecast item)
        {
            Item = item;
        }

        public WeatherForecast Item { get; }
    }
}
