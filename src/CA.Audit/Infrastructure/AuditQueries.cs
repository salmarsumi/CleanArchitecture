using CA.Audit.Application.Audit;
using Microsoft.EntityFrameworkCore;

namespace CA.Audit.Infrastructure
{
    public class AuditQueries : IAuditQueries
    {
        private readonly IAuditDbContext _context;

        public AuditQueries(IAuditDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<AuditDto>> GetAllAuditAsync(CancellationToken cancellationToken = default)
        {
            return
                await _context.AuditEntries.AsNoTracking().Select(x => new AuditDto
                {
                    CorrelationId = x.CorrelationId,
                    Action = x.Action,
                    Id = x.Id,
                    Object = x.Object,
                    Source = x.Source,
                    TimeStamp = x.TimeStamp,
                    Username = x.Username
                })
                .OrderByDescending(x => x.TimeStamp)
                .ToListAsync(cancellationToken);
        }

        public async Task<AuditDetailsDto> GetAuditAsync(int id, CancellationToken cancellationToken = default)
        {
            return
                await _context.AuditEntries.AsNoTracking()
                    .Where(x => x.Id == id)
                    .Select(x => new AuditDetailsDto
                    {
                        Id = x.Id,
                        Action = x.Action,
                        Browser = x.Browser,
                        CorrelationId = x.CorrelationId,
                        IPAddress = x.IPAddress,
                        Key = x.Key,
                        NewValue = x.NewValue,
                        Object = x.Object,
                        OldValue = x.OldValue,
                        Source = x.Source,
                        TimeStamp = x.TimeStamp,
                        UserId = x.UserId,
                        Username = x.Username
                    })
                    .FirstOrDefaultAsync(cancellationToken);
        }
    }
}
