using Microsoft.EntityFrameworkCore.Storage;

namespace CA.Common.EF
{
    /// <summary>
    /// Defines transactional behavior functionality for all
    /// DbContext classes implementing it.This interface
    /// is to manage transactions through out the system.
    /// </summary>
    public interface ITransactionalDbContext
    {
        bool HasActiveTransaction { get; }

        /// <summary>
        /// Creates an instance of the configured <see cref="IExecutionStrategy" />.
        /// See <see href="https://aka.ms/efcore-docs-connection-resiliency">Connection resiliency and database retries</see> for more information.
        /// </summary>
        /// <returns>An <see cref="IExecutionStrategy" /> instance.</returns>
        IExecutionStrategy CreateExecutionStrategy();

        /// <summary>
        /// Get the active transaction instance for the current Db Context.
        /// </summary>
        /// <returns>An <see cref="IDbContextTransaction"/> instance.</returns>
        IDbContextTransaction GetCurrentTransaction();

        /// <summary>
        /// Start a new transaction for the current Db Context.
        /// </summary>
        /// <returns>An <see cref="IDbContextTransaction"/> instance.</returns>
        Task<IDbContextTransaction> BeginTransactionAsync();

        /// <summary>
        /// Commits all changes made to the database in the current transaction asynchronously.
        /// </summary>
        /// <param name="transaction">The <see cref="IDbContextTransaction"/> instance to be committed.</param>
        /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
        Task CommitTransactionAsync(IDbContextTransaction transaction);

        /// <summary>
        /// Rollback all changes made to the database in the current transaction asynchronously.
        /// </summary>
        /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
        Task RollbackTransaction();
    }
}
