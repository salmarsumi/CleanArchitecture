using CA.Common.Authorization.PolicyRuntime;

namespace CA.Authorization.PolicyStore
{
    public interface IPolicyReader
    {
        Task<Policy> ReadPolicyAsync();
    }
}
