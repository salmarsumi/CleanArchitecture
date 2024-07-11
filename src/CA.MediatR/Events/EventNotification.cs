using CA.Common.SeedWork;
using MediatR;

namespace CA.MediatR.Events
{
    /// <summary>
    /// A wrapper class around domain events to make them publishable through 
    /// the MediatR pipeline by implementing the INotification interface.
    /// </summary>
    /// <typeparam name="T">The type of the domain event.</typeparam>
    public class EventNotification <T> : INotification where T : DomainEvent
    {
        public EventNotification(T domainEvent)
        {
            DomainEvent = domainEvent;
        }

        public T DomainEvent { get; }
    }
}
