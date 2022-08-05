namespace CA.Audit.Application.Access
{
    public interface IAccessQueries
    {
        Task<IEnumerable<AccessDto>> GetAllAccessAsync(CancellationToken cancellationToken = default);
        Task<AccessDetailsDto> GetAccessAsync(int id, CancellationToken cancellationToken = default);
    }
}
