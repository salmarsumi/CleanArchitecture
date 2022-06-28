using CA.Common.SeedWork;
using CA.Common.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System.Data;

namespace CA.Common.EF
{
    public abstract class BaseTransactionalDbContext<T> : BaseDbContext<T>, ITransactionalDbContext where T : DbContext
    {
        private IDbContextTransaction _currentTransaction;

        protected BaseTransactionalDbContext(DbContextOptions<T> options, ICurrentUserService currentUserService, IDomainEventService domainEventService)
            : base(options, currentUserService, domainEventService)
        {
        }

        public bool HasActiveTransaction => _currentTransaction is not null;

        public async Task<IDbContextTransaction> BeginTransactionAsync()
        {
            if (_currentTransaction is not null) return null;

            _currentTransaction = await Database.BeginTransactionAsync(IsolationLevel.ReadCommitted);

            return _currentTransaction;
        }

        public async Task CommitTranasctionAsync(IDbContextTransaction transaction)
        {
            if (transaction == null) throw new ArgumentNullException(nameof(transaction));
            if (transaction != _currentTransaction) throw new InvalidOperationException($"Transaction {transaction.TransactionId} is not current");

            try
            {
                await SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch
            {
                await RollbackTransaction();
                throw;
            }
            finally
            {
                if (_currentTransaction != null)
                {
                    _currentTransaction.Dispose();
                    _currentTransaction = null;
                }
            }
        }

        public IExecutionStrategy CreateExecutionStrategy()
        {
            return Database.CreateExecutionStrategy();
        }

        public IDbContextTransaction GetCurrentTransaction() => _currentTransaction;

        public async Task RollbackTransaction()
        {
            try
            {
                await _currentTransaction?.RollbackAsync();
            }
            finally
            {
                if (_currentTransaction != null)
                {
                    _currentTransaction.Dispose();
                    _currentTransaction = null;
                }
            }
        }
    }
}
