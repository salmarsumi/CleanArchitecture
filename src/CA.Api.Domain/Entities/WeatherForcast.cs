using CA.Common.SeedWork;

namespace CA.Api.Domain.Entities
{
    public class WeatherForcast : EntityBase<int>
    {
        public DateTime Date { get; set; }
        public int TemperatureC { get; set; }
        public string Summary { get; set; }
    }
}
