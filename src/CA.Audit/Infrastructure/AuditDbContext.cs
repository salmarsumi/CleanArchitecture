using CA.Audit.Domain;
using Microsoft.EntityFrameworkCore;

namespace CA.Audit.Infrastructure
{
    public class AuditDbContext : DbContext, IAuditDbContext
    {
        public AuditDbContext(DbContextOptions<AuditDbContext> options)
           : base(options)
        { }

        public DbSet<AuditEntryEntity> AuditEntries { get; set; }
        public DbSet<AccessEntryEntity> AccessEntries { get; set; }
    }

    public interface IAuditDbContext
    {
        DbSet<AuditEntryEntity> AuditEntries { get; }
        DbSet<AccessEntryEntity> AccessEntries { get; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}
