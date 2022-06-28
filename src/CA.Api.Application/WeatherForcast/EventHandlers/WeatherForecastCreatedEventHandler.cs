using CA.Api.Domain.Events;
using CA.MediatR.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CA.Api.Application.WeatherForcast.EventHandlers
{
    public class WeatherForecastCreatedEventHandler : INotificationHandler<EventNotification<WeatherForecastCreatedEvent>>
    {
        private readonly ILogger<WeatherForecastCreatedEventHandler> _logger;

        public WeatherForecastCreatedEventHandler(ILogger<WeatherForecastCreatedEventHandler> logger)
        {
            _logger = logger;
        }

        public Task Handle(EventNotification<WeatherForecastCreatedEvent> notification, CancellationToken cancellationToken)
        {
            var @event = notification.DomainEvent;

            _logger.LogInformation("----- Handling Event {DomainEvent}", @event.GetType().Name);

            return Task.CompletedTask;
        }
    }
}
