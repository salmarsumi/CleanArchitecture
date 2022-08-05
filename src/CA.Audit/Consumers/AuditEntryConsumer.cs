using CA.Audit.Domain;
using CA.Audit.Infrastructure;
using CA.Common.Contracts.Audit;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace CA.Audit.Consumers
{
    public class AuditEntryConsumer : IConsumer<AuditEntry>
    {
        private readonly AuditDbContext _context;
        private readonly ILogger<AuditEntryConsumer> _logger;

        public AuditEntryConsumer(
            AuditDbContext context,
            ILogger<AuditEntryConsumer> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<AuditEntry> context)
        {
            _logger.LogInformation("Received Entry: {@Entry}", context.Message);
            var strategy = _context.Database.CreateExecutionStrategy();

            // Using the configured retry policy
            await strategy.ExecuteAsync(async () =>
            {
                _context.Add(AuditEntryEntity.CreateFromAuditEntry(context.Message));
                await _context.SaveChangesAsync();
            });
        }
    }
}
