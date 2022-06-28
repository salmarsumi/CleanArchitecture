using CA.Common.SeedWork;
using MediatR;

namespace CA.MediatR.Events
{
    public class EventNotification <T> : INotification where T : DomainEvent
    {
        public EventNotification(T domainEvent)
        {
            DomainEvent = domainEvent;
        }

        public T DomainEvent { get; }
    }
}
