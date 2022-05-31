using FluentValidation;

namespace CA.Api.Application.WeatherForcast.Commands.Delete
{
    public class DeleteWeatherForcastValidator : AbstractValidator<DeleteWeatherForcastCommand>
    {
        public DeleteWeatherForcastValidator()
        {
            RuleFor(command => command.Id).NotEmpty();
        }
    }
}
