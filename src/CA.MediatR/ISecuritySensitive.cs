namespace CA.MediatR
{
    public interface ISecuritySensitive<T>
    {
        T GetSafeCopy();
    }
}
