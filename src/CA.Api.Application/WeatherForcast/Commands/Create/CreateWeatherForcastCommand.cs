using CA.Api.Domain.Entities;
using CA.Api.Domain.Events;
using CA.Api.Domain.Interfaces;
using CA.MediatR;
using MediatR;

namespace CA.Api.Application.WeatherForcast.Commands.Create
{
    public class CreateWeatherForcastCommand : IRequest<RequestResult<int>>, ITransactionalRequest
    {
        public DateTime Date { get; set; }
        public int TemperatureC { get; set; }
        public string Summary { get; set; }
    }

    public class CreateWeatherForcastCommandHandler : IRequestHandler<CreateWeatherForcastCommand, RequestResult<int>>
    {
        private readonly IWeatherForecastRepository _repository;

        public CreateWeatherForcastCommandHandler(IWeatherForecastRepository repository)
        {
            _repository = repository;
        }

        public async Task<RequestResult<int>> Handle(CreateWeatherForcastCommand request, CancellationToken cancellationToken)
        {
            var entity = new WeatherForecast
            {
                Date = request.Date,
                TemperatureC = request.TemperatureC,
                Summary = request.Summary
            };

            entity.AddDomainEvent(new WeatherForecastCreatedEvent(entity));

            _repository.Add(entity);

            await _repository.SaveChangesAsync(cancellationToken);

            return RequestResult<int>.Succeeded(entity.Id);
        }
    }
}
