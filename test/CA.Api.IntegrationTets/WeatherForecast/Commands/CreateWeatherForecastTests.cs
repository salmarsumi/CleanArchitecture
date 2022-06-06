using CA.Api.Application.Interfaces;
using CA.Api.Application.WeatherForcast.Commands.Create;
using CA.Api.Domain.Entities;
using CA.Api.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CA.Api.IntegrationTets.WeatherForecast.Commands
{
    public class CreateWeatherForecastTests : RequestHandlerTestBase<CreateWeatherForcastCommandHandler>
    {
        [Fact]
        public async Task Handle_ReturnsId_WhenCommandIsValid()
        {
            // Arrange
            var currentUser = new FakeCurrentUserService();
            var command = new CreateWeatherForcastCommand
            {
                Date = DateTime.Now,
                Summary = "Create weather forecast test",
                TemperatureC = 50
            };
            using ApiDbContext context = CreateApiContext();

            // Act
            int id = await _handler.Handle(command, CancellationToken.None);

            WeatherForcast entity = await context.WeatherForcasts
                .AsNoTracking()
                .Where(x => x.Id == id).FirstOrDefaultAsync();

            // Assert
            Assert.True(id > 0);
            Assert.NotNull(entity);
            Assert.Equal(command.Date, entity.Date);
            Assert.Equal(command.Summary, entity.Summary);
            Assert.Equal(command.TemperatureC, entity.TemperatureC);
            Assert.Equal(currentUser.GetUsername(), entity.CreatedBy);
            Assert.Equal(currentUser.GetUsername(), entity.LastModifiedBy);
            Assert.NotEqual(default, entity.Created);
            Assert.NotEqual(default, entity.LastModified);
            Assert.Equal(0, entity.RowVersion);
        }
    }
}
