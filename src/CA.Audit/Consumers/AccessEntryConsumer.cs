using CA.Audit.Domain;
using CA.Audit.Infrastructure;
using CA.Common.Contracts.Audit;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace CA.Audit.Consumers
{
    public class AccessEntryConsumer : IConsumer<AccessEntry>
    {
        private readonly AuditDbContext _context;
        private readonly ILogger<AccessEntryConsumer> _logger;

        public AccessEntryConsumer(
            AuditDbContext context,
            ILogger<AccessEntryConsumer> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<AccessEntry> context)
        {
            _logger.LogInformation("Received Entry: {@Entry}", context.Message);
            var strategy = _context.Database.CreateExecutionStrategy();

            // Using the configured retry policy
            await strategy.ExecuteAsync(async () =>
            {
                _context.Add(AccessEntryEntity.CreateFromAccessEntry(context.Message));
                await _context.SaveChangesAsync();
            });
        }
    }
}
