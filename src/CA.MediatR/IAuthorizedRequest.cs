namespace CA.MediatR
{
    public interface IAuthorizedRequest
    {
        IEnumerable<string> GetRequiredPermissions();
    }
}
