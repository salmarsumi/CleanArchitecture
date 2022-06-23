using CA.Api.Domain.Interfaces;
using CA.Common.SeedWork;

namespace CA.Api.Domain.Entities
{
    public class WeatherForecast : EntityBase<int>, IAggregate
    {
        public DateTime Date { get; set; }
        public int TemperatureC { get; set; }
        public string Summary { get; set; }
    }
}
