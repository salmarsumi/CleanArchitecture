using CA.Api.Infrastructure.Data;
using CA.Api.Infrastructure.Repositories;

namespace CA.Api.IntegrationTets
{
    public class RequestHandlerTestBase<T>
        where T : class
    {
        protected readonly T _handler;

        public RequestHandlerTestBase()
        {
            IApiDbContext context = CreateApiContext();
            var respository = new WeatherForecastRepository(CreateApiContext());
            _handler = Activator.CreateInstance(typeof(T), respository) as T;
        }

        protected ApiDbContext CreateApiContext()
        {
            return new ApiDbContextFactory().CreateDbContext(Array.Empty<string>());
        }
    }
}
