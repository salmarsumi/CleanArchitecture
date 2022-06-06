using CA.Api.Application.WeatherForcast.Commands.Create;
using FluentValidation;

namespace CA.Api.UnitTests.Validation.WeatherForecast
{
    public class CreateWeatherForecastValidationTets : RequestValidationBase<CreateWeatherForcastCommand, int, CreateWeatherForcastValidator>
    {
        // Test data
        public static IEnumerable<object[]> _emptyCreateForecast = new[]
        {
            new CreateWeatherForcastCommand[] { new() { Date = default, Summary = null } },
            new CreateWeatherForcastCommand[] { new() { Date = default, Summary = "" } },
            new CreateWeatherForcastCommand[] { new() { Date = default, Summary = " " } }
        };

        [Theory]
        [MemberData(nameof(_emptyCreateForecast))]
        public async Task Validate_ThrowsException_WhenEmptyAttributes(CreateWeatherForcastCommand command)
        {
            // Arrange
            // Act
            // Assert
            var exception = await Assert.ThrowsAsync<ValidationException>(() => _validatorBehavior.Handle(command, CancellationToken.None, _next.Object));
        }

        [Fact]
        public async Task Validate_ThrowsException_WhenSummaryIsNotAlphaNumeric()
        {
            // Arrange
            var command = new CreateWeatherForcastCommand()
            {
                Date = DateTime.Now,
                Summary = "<script>",
                TemperatureC = 50
            };

            // Act
            // Assert
            var exception = await Assert.ThrowsAsync<ValidationException>(() => _validatorBehavior.Handle(command, CancellationToken.None, _next.Object));
        }

        [Fact]
        public async Task Validate_PassValidation_WhenObjectIsValid()
        {
            // Arrange
            var command = new CreateWeatherForcastCommand()
            {
                Date = DateTime.Now,
                Summary = "Some Summary",
                TemperatureC = 50
            };

            // Act
            // Assert
            await _validatorBehavior.Handle(command, CancellationToken.None, _next.Object);
        }
    }
}
