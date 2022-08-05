namespace CA.Audit.Application.Audit
{
    public interface IAuditQueries
    {
        Task<IEnumerable<AuditDto>> GetAllAuditAsync(CancellationToken cancellationToken = default);
        Task<AuditDetailsDto> GetAuditAsync(int id, CancellationToken cancellationToken = default);
    }
}
