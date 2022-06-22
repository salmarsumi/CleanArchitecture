using CA.Api.Application.WeatherForcast.Commands.Delete;
using CA.MediatR;
using FluentValidation;
using MediatR;

namespace CA.Api.UnitTests.Validation.WeatherForecast
{
    public class DeleteWeatherForecastValidationTests : RequestValidationBase<DeleteWeatherForcastCommand, RequestResult<Unit>, DeleteWeatherForcastValidator>
    {
        [Fact]
        public async Task Validate_ThrowsException_WhenEmptyAttributes()
        {
            // Arrange
            var command = new DeleteWeatherForcastCommand { Id = 0 };

            // Act
            RequestResult<Unit> result = await _validatorBehavior.Handle(command, CancellationToken.None, _next);

            // Assert
            Assert.True(result.IsNotValid);
            Assert.False(result.Success);
        }

        [Fact]
        public async Task Validate_ReturnsNotValid_WhenObjectIsValid()
        {
            // Arrange
            var command = new DeleteWeatherForcastCommand { Id = 1 };

            // Act
            RequestResult<Unit> result = await _validatorBehavior.Handle(command, CancellationToken.None, _next);

            // Assert
            Assert.True(result.Success);
            Assert.False(result.IsNotValid);
        }
    }
}
