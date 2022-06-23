using CA.Api.Application.WeatherForcast.Queries;
using CA.Api.Domain.Interfaces;
using CA.Api.Infrastructure.Data;
using CA.Api.Infrastructure.Queries;
using CA.Api.Infrastructure.Repositories;
using System.Reflection;

namespace CA.Api.IntegrationTets
{
    public class RequestHandlerTestBase<T>
        where T : class
    {
        protected readonly T _handler;

        public RequestHandlerTestBase()
        {
            var parameters = new List<object>();
            ConstructorInfo constructor = typeof(T).GetConstructors().First();
            
            foreach(ParameterInfo p in constructor.GetParameters())
            {
                if(p.ParameterType == typeof(IWeatherForecastRepository))
                {
                    var repository = new WeatherForecastRepository(CreateApiContext());
                    parameters.Add(repository);
                }
                else if(p.ParameterType == typeof(IWeatherForecastQueries))
                {
                    var query = new WeatherForecastQueries(CreateApiContext());
                    parameters.Add(query);
                }

            }

            _handler = Activator.CreateInstance(typeof(T), parameters.ToArray()) as T;
        }

        protected ApiDbContext CreateApiContext()
        {
            return new ApiDbContextFactory().CreateDbContext(Array.Empty<string>());
        }
    }
}
