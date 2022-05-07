﻿using CA.Common.SeedWork;
using CA.Common.Services;
using Microsoft.EntityFrameworkCore;

namespace CA.Common.EF
{
    public abstract class BaseDbContext<T> : DbContext where T : DbContext
    {
        protected readonly ICurrentUserService _currentUserService;

        protected BaseDbContext(DbContextOptions<T> options, ICurrentUserService currentUserService)
            : base(options)
        {
            _currentUserService = currentUserService;
        }

        public override int SaveChanges()
        {
            Audit();
            Concurrency();
            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            Audit();
            Concurrency();
            return base.SaveChangesAsync(cancellationToken);
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
                if (string.IsNullOrEmpty(auditableEntity.CreatedBy))
                {
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
                }

                if (string.IsNullOrEmpty(auditableEntity.LastModifiedBy))
                {
                    // In any case we always want to set the properties
                    // Modified At and Modified By
                    auditableEntity.LastModified = DateTime.UtcNow;
                    auditableEntity.LastModifiedBy = username;
                }
            }
        }

        private void Concurrency()
        {
            foreach (var entry in ChangeTracker.Entries().Where(e => e.Entity is IConcurrency && e.State == EntityState.Modified))
            {
                ((IConcurrency)entry.Entity).IncrementVersion();
            }
        }
    }
}
