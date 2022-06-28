﻿using CA.Common.SeedWork;
using MediatR;

namespace CA.MediatR.Events
{
    public class DomainEventService : IDomainEventService
    {
        private readonly IPublisher _mediator;

        public DomainEventService(IPublisher mediator)
        {
            _mediator = mediator;
        }

        public async Task Publish(DomainEvent domainEvent)
        {
            await _mediator.Publish(GetNotificationFromDomainEvent(domainEvent));
        }

        private INotification GetNotificationFromDomainEvent(DomainEvent domainEvent)
        {
            return (INotification)Activator.CreateInstance(
                typeof(EventNotification<>).MakeGenericType(domainEvent.GetType()), domainEvent)!;
        }
    }
}
