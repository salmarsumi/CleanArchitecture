﻿using CA.MediatR;
using MediatR;

namespace CA.Api.Application.WeatherForcast.Queries
{
    /// <summary>
    /// Query
    /// </summary>
    public class GetAllWeatherForcastQuery : IRequest<RequestResult<IEnumerable<WeatherForecastDto>>>
    {
    }

    /// <summary>
    /// Handler
    /// </summary>
    public class GetAllWeatherForcastQueryHandler : IRequestHandler<GetAllWeatherForcastQuery, RequestResult<IEnumerable<WeatherForecastDto>>>
    {
        private readonly IWeatherForecastQueries _query;

        public GetAllWeatherForcastQueryHandler(IWeatherForecastQueries query)
        {
            _query = query;
        }

        public async Task<RequestResult<IEnumerable<WeatherForecastDto>>> Handle(GetAllWeatherForcastQuery request, CancellationToken cancellationToken)
        {
            IEnumerable<WeatherForecastDto> result = await _query.GetAllWeatherForecastAsync(cancellationToken);
            return RequestResult<IEnumerable<WeatherForecastDto>>.Succeeded(result);
        }
    }
}
