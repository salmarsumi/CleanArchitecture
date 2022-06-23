using MediatR;

namespace CA.Api.Application.WeatherForcast.Queries
{
    public class GetAllWeatherForcastQuery : IRequest<IEnumerable<WeatherForecastDto>>
    {
    }

    public class GetAllWeatherForcastQueryHandler : IRequestHandler<GetAllWeatherForcastQuery, IEnumerable<WeatherForecastDto>>
    {
        private readonly IWeatherForecastQueries _query;

        public GetAllWeatherForcastQueryHandler(IWeatherForecastQueries query)
        {
            _query = query;
        }

        public async Task<IEnumerable<WeatherForecastDto>> Handle(GetAllWeatherForcastQuery request, CancellationToken cancellationToken)
        {
            return await _query.GetAllWeatherForecastAsync(cancellationToken);
        }
    }
}
