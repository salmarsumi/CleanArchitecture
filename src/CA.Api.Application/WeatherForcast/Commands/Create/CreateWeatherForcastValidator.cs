using FluentValidation;

namespace CA.Api.Application.WeatherForcast.Commands.Create
{
    public class CreateWeatherForcastValidator : AbstractValidator<CreateWeatherForcastCommand>
    {
        public CreateWeatherForcastValidator()
        {
            RuleFor(command => command.Date).NotEmpty();
            RuleFor(command => command.Summary).NotEmpty().Matches("^[a-zA-Zء-ي0-9 ]+$"); // Alpha numeric only
        }
    }
}
