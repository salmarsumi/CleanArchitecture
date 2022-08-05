using CA.Api.Application.WeatherForcast.Commands.Create;
using CA.Api.Domain.Events;
using CA.Common.Contracts.Audit;
using CA.Common.Services;
using CA.MediatR.Events;
using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CA.Api.Application.WeatherForcast.EventHandlers
{
    public class WeatherForecastCreatedEventHandler : INotificationHandler<EventNotification<WeatherForecastCreatedEvent>>
    {
        private readonly ICurrentRequestService _currentRequest;
        private readonly ILogger<WeatherForecastCreatedEventHandler> _logger;
        private readonly IPublishEndpoint _publishEndpoint;

        public WeatherForecastCreatedEventHandler(
            ICurrentRequestService currentRequest,
            ILogger<WeatherForecastCreatedEventHandler> logger,
            IPublishEndpoint publishEndpoint = null)
        {
            _currentRequest = currentRequest;
            _logger = logger;
            _publishEndpoint = publishEndpoint;
        }

        public async Task Handle(EventNotification<WeatherForecastCreatedEvent> notification, CancellationToken cancellationToken)
        {
            var @event = notification.DomainEvent;

            _logger.LogInformation("----- Handling Event {DomainEvent}", @event.GetType().Name);

            if(_publishEndpoint is not null)
            {
                object old = null;
                var entry = new
                {
                    CorrelationId = _currentRequest.GetCorrelationId(),
                    UserId = _currentRequest.GetUserId(),
                    Username = _currentRequest.GetUsername(),
                    Action = "Create Weather Forecast",
                    Key = @event.Entity.Id,
                    Source = "Api",
                    Object = "Weather Forecast",
                    Browser = _currentRequest.GetClientBrowser(),
                    IPAddress = _currentRequest.GetClientIPAddress(),
                    TimeStamp = DateTime.UtcNow,
                    OldValue = old,
                    NewValue = new
                    {
                        @event.Entity.Date,
                        @event.Entity.Summary,
                        @event.Entity.TemperatureC,
                        @event.Entity.RowVersion
                    }
                };

                _logger.LogInformation("----- Sending AuditEntry: {@AuditEntry}", entry);
                await _publishEndpoint.Publish<AuditEntry>(entry, cancellationToken);
            }
        }
    }
}
