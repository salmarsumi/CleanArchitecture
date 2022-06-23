using CA.Api.Application.WeatherForcast.Queries;
using CA.Api.Infrastructure.Data;
using CA.MediatR;
using Entities = CA.Api.Domain.Entities;

namespace CA.Api.IntegrationTets.WeatherForecast.Queries
{
    public class GetAllWeatherForcastTests : RequestHandlerTestBase<GetAllWeatherForcastQueryHandler>
    {
        [Fact]
        public async Task Handle_ReturnsWeatherForecasts_WhenCommandIsValid()
        {
            // Arrange
            IApiDbContext context = CreateApiContext();
            var entity = new Entities.WeatherForecast
            {
                Created = DateTime.Now,
                CreatedBy = "Test User",
                Date = DateTime.Now,
                LastModified = DateTime.Now,
                LastModifiedBy = "Test User",
                Summary = "Summary",
                TemperatureC = 50
            };
            context.WeatherForcasts.Add(entity);
            await context.SaveChangesAsync(CancellationToken.None);

            var query = new GetAllWeatherForcastQuery();

            // Act
            RequestResult<IEnumerable<WeatherForecastDto>> result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.True(result.Success);
            Assert.Contains(result.Result, x => x.Id == entity.Id);
        }
    }
}
