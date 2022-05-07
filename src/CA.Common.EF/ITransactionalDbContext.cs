using Microsoft.EntityFrameworkCore.Storage;

namespace CA.Common.EF
{
    public interface ITransactionalDbContext
    {
        bool HasActiveTransaction { get; }

        IExecutionStrategy CreateExecutionStrategy();
        IDbContextTransaction GetCurrentTransaction();
        Task<IDbContextTransaction> BeginTransactionAsync();
        Task CommitTranasctionAsync(IDbContextTransaction transaction);
        Task RollbackTransaction();
    }
}
