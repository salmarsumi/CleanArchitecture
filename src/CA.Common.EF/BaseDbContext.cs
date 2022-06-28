using CA.Common.Exceptions;
using CA.Common.SeedWork;
using CA.Common.Services;
using Microsoft.EntityFrameworkCore;

namespace CA.Common.EF
{
    public abstract class BaseDbContext<T> : DbContext where T : DbContext
    {
        protected readonly ICurrentUserService _currentUserService;
        private readonly IDomainEventService _domainEventService;

        protected BaseDbContext(DbContextOptions<T> options, ICurrentUserService currentUserService, IDomainEventService domainEventService)
            : base(options)
        {
            _currentUserService = currentUserService;
            _domainEventService = domainEventService;
        }

        public override int SaveChanges()
        {
            Audit();
            Concurrency();

            try
            {
                var events = ChangeTracker.Entries<IHasDomainEvents>()
                    .Select(x => x.Entity.Events)
                    .SelectMany(x => x)
                    .Where(domainEvent => !domainEvent.IsPublished)
                    .ToArray();

                int result = base.SaveChanges();
                PublishEvents(events).GetAwaiter().GetResult();
                return result;
            }
            catch (DbUpdateConcurrencyException ex)
            {
                throw new ConcurrencyException(ex.Message, ex);
            }
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            Audit();
            Concurrency();

            try
            {
                var events = ChangeTracker.Entries<IHasDomainEvents>()
                    .Select(x => x.Entity.Events)
                    .SelectMany(x => x)
                    .Where(domainEvent => !domainEvent.IsPublished)
                    .ToArray();

                int result = await base.SaveChangesAsync(cancellationToken);
                await PublishEvents(events);
                return result;
            }
            catch (DbUpdateConcurrencyException ex)
            {
                throw new ConcurrencyException(ex.Message, ex);
            }
        }

        private void Audit()
        {
            string username = _currentUserService.GetUsername();
            IAuditable auditableEntity;
            // Iterate through IAuditable entities with added or modified state
            foreach (var entry in ChangeTracker.Entries().Where(e => e.Entity is IAuditable && (e.State == EntityState.Added || e.State == EntityState.Modified)))
            {
                // If the entity state is Added let's set
                // the CreatedAt and CreatedBy properties
                auditableEntity = (IAuditable)entry.Entity;
                if (entry.State == EntityState.Added)
                {
                    auditableEntity.Created = DateTime.UtcNow;
                    auditableEntity.CreatedBy = username;
                }
                else
                {
                    // If the state is Modified then we don't want
                    // to modify the Created At and Created By properties
                    // so we set their state as IsModified to false
                    Entry(auditableEntity).Property(p => p.Created).IsModified = false;
                    Entry(auditableEntity).Property(p => p.CreatedBy).IsModified = false;
                }

                // In any case we always want to set the properties
                // Modified At and Modified By
                auditableEntity.LastModified = DateTime.UtcNow;
                auditableEntity.LastModifiedBy = username;
            }
        }

        private void Concurrency()
        {
            foreach (var entry in ChangeTracker.Entries().Where(e => e.Entity is IConcurrency && e.State == EntityState.Modified))
            {
                ((IConcurrency)entry.Entity).IncrementVersion();
            }
        }

        private async Task PublishEvents(DomainEvent[] events)
        {
            foreach (DomainEvent @event in events)
            {
                if (!@event.IsPublished)
                {
                    await _domainEventService.Publish(@event);
                    @event.SetPublished();
                }
            }
        }
    }
}
