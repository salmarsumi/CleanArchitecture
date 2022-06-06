using CA.Api.Application.WeatherForcast.Commands.Delete;
using FluentValidation;
using MediatR;

namespace CA.Api.UnitTests.Validation.WeatherForecast
{
    public class DeleteWeatherForecastValidationTests : RequestValidationBase<DeleteWeatherForcastCommand, Unit, DeleteWeatherForcastValidator>
    {
        [Fact]
        public async Task Validate_ThrowsException_WhenEmptyAttributes()
        {
            // Arrange
            var command = new DeleteWeatherForcastCommand { Id = 0 };

            // Act
            // Assert
            var exception = await Assert.ThrowsAsync<ValidationException>(() => _validatorBehavior.Handle(command, CancellationToken.None, _next.Object));
        }

        [Fact]
        public async Task Validate_PassValidation_WhenObjectIsValid()
        {
            // Arrange
            var command = new DeleteWeatherForcastCommand { Id = 1 };

            // Act
            // Assert
            await _validatorBehavior.Handle(command, CancellationToken.None, _next.Object);
        }
    }
}
