using CA.Api.Application.WeatherForcast.Commands.Delete;
using CA.Api.Domain.Entities;
using CA.Api.Infrastructure.Data;
using CA.Common.Exceptions;
using CA.MediatR;
using MediatR;
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
            RequestResult<Unit> result = await _handler.Handle(command, CancellationToken.None);

            WeatherForcast deleted = await context.WeatherForcasts
                .AsNoTracking()
                .Where(x => x.Id == newEntity.Id).FirstOrDefaultAsync();

            // Assert
            Assert.True(result.Success);
            Assert.Equal(Unit.Value, result.Result);
            Assert.Null(deleted);
        }

        [Fact]
        public async Task Handle_ReturnsNotFound_WhenWeatherForecastNotFound()
        {
            // Arrange
            var command = new DeleteWeatherForcastCommand { Id = 1000 };

            // Act
            RequestResult<Unit> result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.IsNotFound);
            Assert.False(result.Success);
        }
    }
}
