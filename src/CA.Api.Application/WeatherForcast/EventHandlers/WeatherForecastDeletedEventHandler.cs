using CA.Api.Domain.Events;
using CA.MediatR.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CA.Api.Application.WeatherForcast.EventHandlers
{
    public class WeatherForecastDeletedEventHandler : INotificationHandler<EventNotification<WeatherForecastDeletedEvent>>
    {
        private readonly ILogger<WeatherForecastDeletedEventHandler> _logger;

        public WeatherForecastDeletedEventHandler(ILogger<WeatherForecastDeletedEventHandler> logger)
        {
            _logger = logger;
        }

        public Task Handle(EventNotification<WeatherForecastDeletedEvent> notification, CancellationToken cancellationToken)
        {
            var @event = notification.DomainEvent;

            _logger.LogInformation("----- Handling Event {DomainEvent}", @event.GetType().Name);

            return Task.CompletedTask;
        }
    }
}
