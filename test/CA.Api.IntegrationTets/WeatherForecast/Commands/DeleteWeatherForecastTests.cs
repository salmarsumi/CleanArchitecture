using CA.Api.Application.WeatherForcast.Commands.Delete;
using CA.Api.Domain.Entities;
using CA.Api.Infrastructure.Data;
using CA.Common.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace CA.Api.IntegrationTets.WeatherForecast.Commands
{
    public class DeleteWeatherForecastTests : RequestHandlerTestBase<DeleteWeatherForcastCommandHandler>
    {
        [Fact]
        public async Task Handle_DeletesWeatherForecast_WhenCommandIsValid()
        {
            // Arrange
            using ApiDbContext context = CreateApiContext();
            var newEntity = new WeatherForcast
            {
                Date = DateTime.Now,
                Summary = "Delete weather forecast test",
                TemperatureC = 50
            };
            context.Add(newEntity);
            await context.SaveChangesAsync();

            var command = new DeleteWeatherForcastCommand { Id = newEntity.Id };

            // Act
            await _handler.Handle(command, CancellationToken.None);

            WeatherForcast deleted = await context.WeatherForcasts
                .AsNoTracking()
                .Where(x => x.Id == newEntity.Id).FirstOrDefaultAsync();

            // Assert
            Assert.Null(deleted);
        }

        [Fact]
        public async Task Handle_ThrowsException_WhenWeatherForecastNotFound()
        {
            // Arrange
            var command = new DeleteWeatherForcastCommand { Id = 1000 };

            // Act
            // Assert
            await Assert.ThrowsAsync<NotFoundException>(() => _handler.Handle(command, CancellationToken.None));
        }
    }
}
