using CA.Api.Application.WeatherForcast.Commands.Create;
using CA.MediatR;
using FluentValidation;

namespace CA.Api.UnitTests.Validation.WeatherForecast
{
    public class CreateWeatherForecastValidationTets : RequestValidationBase<CreateWeatherForcastCommand, RequestResult<int>, CreateWeatherForcastValidator>
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
        public async Task Validate_ReturnsNotValid_WhenEmptyAttributes(CreateWeatherForcastCommand command)
        {
            // Arrange
            // Act
            RequestResult<int> result = await _validatorBehavior.Handle(command, CancellationToken.None, _next);

            // Assert
            Assert.True(result.IsNotValid);
            Assert.False(result.Success);
        }

        [Fact]
        public async Task Validate_ReturnsNotValid_WhenSummaryIsNotAlphaNumeric()
        {
            // Arrange
            var command = new CreateWeatherForcastCommand()
            {
                Date = DateTime.Now,
                Summary = "<script>",
                TemperatureC = 50
            };

            // Act
            RequestResult<int> result = await _validatorBehavior.Handle(command, CancellationToken.None, _next);

            // Assert
            Assert.True(result.IsNotValid);
            Assert.False(result.Success);
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
            RequestResult<int> result = await _validatorBehavior.Handle(command, CancellationToken.None, _next);
            
            // Assert
            Assert.True(result.Success);
            Assert.False(result.IsNotValid);
        }
    }
}
