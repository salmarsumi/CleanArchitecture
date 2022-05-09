using SMD.Security.Authorization.PolicyRuntime;
using SMD.Security.Authorization.Store;

namespace CA.Authorization
{
    public class PolicyReader : IPolicyReader
    {
        private readonly Policy _policy = new(
            new Role[]
            {
                new() 
                { 
                    Name = "Admin",
                    Subjects = new[] { "2" }
                },
            },
            new Permission[] 
            { 
                new() 
                {
                    Name = "ViewWeather",
                    Roles = new[] { "Admin" }
                },
                new()
                {
                    Name = "CreateWeather",
                    Roles = new[] { "Admin" }
                }
            });

        public Task<Policy> ReadPolicyAsync()
        {
            return Task.FromResult(_policy);
        }
    }
}
