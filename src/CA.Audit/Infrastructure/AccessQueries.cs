using CA.Audit.Application.Access;
using Microsoft.EntityFrameworkCore;

namespace CA.Audit.Infrastructure
{
    public class AccessQueries : IAccessQueries
    {
        private readonly IAuditDbContext _context;

        public AccessQueries(IAuditDbContext context)
        {
            _context = context;
        }

        public async Task<AccessDetailsDto> GetAccessAsync(int id, CancellationToken cancellationToken = default)
        {
            return
                await _context.AccessEntries.AsNoTracking()
                    .Where(x => x.Id == id)
                    .Select(x => new AccessDetailsDto
                    {
                        Id = x.Id,
                        Action = x.Action,
                        Browser = x.Browser,
                        CorrelationId = x.CorrelationId,
                        IPAddress = x.IPAddress,
                        Details = x.Details,
                        Result = x.Result,
                        TimeStamp = x.TimeStamp,
                        UserId = x.UserId,
                        Username = x.Username
                    })
                    .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<IEnumerable<AccessDto>> GetAllAccessAsync(CancellationToken cancellationToken = default)
        {
            return
                await _context.AccessEntries.AsNoTracking().Select(x => new AccessDto
                {
                    Action = x.Action,
                    Id = x.Id,
                    CorrelationId = x.CorrelationId,
                    Result = x.Result,
                    TimeStamp = x.TimeStamp,
                    Username = x.Username
                })
                .OrderByDescending(x => x.TimeStamp)
                .ToListAsync(cancellationToken);
        }
    }
}
