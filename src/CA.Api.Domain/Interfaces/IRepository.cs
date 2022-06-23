using CA.Common.SeedWork;

namespace CA.Api.Domain.Interfaces
{
    public interface IRepository<TEntity, TId> where TEntity : IAggregate
    {
        void Add(TEntity entity);
        void Remove(TEntity entity);
        Task<TEntity> GetAsync(TId id, CancellationToken cancellationToken = default);
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
