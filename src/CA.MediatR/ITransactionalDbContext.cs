﻿using Microsoft.EntityFrameworkCore.Storage;

namespace CA.MediatR
{
    public interface ITransactionalDbContext
    {
        bool HasActiveTransaction { get; }

        IExecutionStrategy CreateExecutionStrategy();
        IDbContextTransaction GetCurrentTransaction();
        Task<IDbContextTransaction> BeginTransactionAsync();
        Task CommitTranasctionAsync(IDbContextTransaction transaction);
        void RollbackTransaction();
    }
}
