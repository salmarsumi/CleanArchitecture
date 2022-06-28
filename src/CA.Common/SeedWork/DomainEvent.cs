namespace CA.Common.SeedWork
{
    public interface IHasDomainEvents
    {
        IEnumerable<DomainEvent> Events { get; }

        void AddDomainEvent(DomainEvent domainEvent);
    }

    public class DomainEvent
    {
        public bool IsPublished { get; private set; }

        public void SetPublished()
        {
            IsPublished = true;
        }
    }

    public interface IDomainEventService
    {
        Task Publish(DomainEvent domainEvent);
    }
}
