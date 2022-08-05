using CA.Api.Domain.Events;
using CA.Common.Contracts.Audit;
using CA.Common.Services;
using CA.MediatR.Events;
using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CA.Api.Application.WeatherForcast.EventHandlers
{
    public class WeatherForecastDeletedEventHandler : INotificationHandler<EventNotification<WeatherForecastDeletedEvent>>
    {
        private readonly ICurrentRequestService _currentRequest;
        private readonly ILogger<WeatherForecastDeletedEventHandler> _logger;
        private readonly IPublishEndpoint _publishEndpoint;

        public WeatherForecastDeletedEventHandler(
            ICurrentRequestService currentRequest,
            ILogger<WeatherForecastDeletedEventHandler> logger,
            IPublishEndpoint publishEndpoint = null)
        {
            _currentRequest = currentRequest;
            _logger = logger;
            _publishEndpoint = publishEndpoint;
        }

        public async Task Handle(EventNotification<WeatherForecastDeletedEvent> notification, CancellationToken cancellationToken)
        {
            var @event = notification.DomainEvent;

            _logger.LogInformation("----- Handling Event {DomainEvent}", @event.GetType().Name);

            if (_publishEndpoint is not null)
            {
                object @new = null;
                var entry = new
                {
                    CorrelationId = _currentRequest.GetCorrelationId(),
                    UserId = _currentRequest.GetUserId(),
                    Username = _currentRequest.GetUsername(),
                    Action = "Delete Weather Forecast",
                    Key = @event.Entity.Id,
                    Source = "Api",
                    Object = "Weather Forecast",
                    Browser = _currentRequest.GetClientBrowser(),
                    IPAddress = _currentRequest.GetClientIPAddress(),
                    TimeStamp = DateTime.UtcNow,
                    NewValue = @new,
                    OldValue = new
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
