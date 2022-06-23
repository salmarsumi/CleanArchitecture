namespace CA.Api.Application.WeatherForcast.Queries
{
    public interface IWeatherForecastQueries
    {
        Task<IEnumerable<WeatherForecastDto>> GetAllWeatherForecastAsync(CancellationToken cancellationToken = default);
    }
}
