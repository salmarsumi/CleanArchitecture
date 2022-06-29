using CA.Api.Domain.Entities;
using CA.Common.SeedWork;

namespace CA.Api.Domain.Events
{
    public class WeatherForecastDeletedEvent : DomainEvent
    {
        public WeatherForecastDeletedEvent(WeatherForecast entity)
        {
            Entity = entity;
        }

        public WeatherForecast Entity { get; }
    }
}
