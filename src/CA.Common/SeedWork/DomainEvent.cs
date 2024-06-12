namespace CA.Common.SeedWork
{
    /// <summary>
    /// Entity related domain event operations.
    /// </summary>
    public interface IHasDomainEvents
    {
        /// <summary>
        /// The collection of events that the current entity has.
        /// </summary>
        IEnumerable<DomainEvent> Events { get; }

        /// <summary>
        /// Adds a new domain event to the events collection.
        /// </summary>
        /// <param name="domainEvent"></param>
        void AddDomainEvent(DomainEvent domainEvent);
    }

    /// <summary>
    /// Represents a domain event.
    /// </summary>
    public abstract class DomainEvent
    {
        /// <summary>
        /// Indicate whether the current event is published. 
        /// </summary>
        public bool IsPublished { get; private set; }

        /// <summary>
        /// Marks the event as published
        /// </summary>
        public void SetPublished()
        {
            IsPublished = true;
        }
    }

    /// <summary>
    /// Domain events operations.
    /// </summary>
    public interface IDomainEventService
    {
        /// <summary>
        /// Publish a specific event.
        /// </summary>
        /// <param name="domainEvent">The domain event instance to be published.</param>
        Task Publish(DomainEvent domainEvent);
    }
}
