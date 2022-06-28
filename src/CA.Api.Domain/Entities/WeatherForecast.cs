using CA.Api.Domain.Interfaces;
using CA.Common.SeedWork;

namespace CA.Api.Domain.Entities
{
    public class WeatherForecast : EntityBase<int>, IAggregate, IHasDomainEvents
    {
        private readonly List<DomainEvent> _events = new();

        public DateTime Date { get; set; }
        public int TemperatureC { get; set; }
        public string Summary { get; set; }
        public IEnumerable<DomainEvent> Events => _events.AsReadOnly();

        public void AddDomainEvent(DomainEvent domainEvent)
        {
            _events.Add(domainEvent);
        }
    }
}
