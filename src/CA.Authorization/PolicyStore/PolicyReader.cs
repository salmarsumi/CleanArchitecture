using CA.Common.Authorization;
using CA.Common.Authorization.PolicyRuntime;

namespace CA.Authorization.PolicyStore
{
    public class PolicyReader : IPolicyReader
    {
        private static readonly Policy _policy = new(
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
                    Name = AppPermissions.ViewWeather,
                    Roles = new[] { "Admin" }
                },
                new()
                {
                    Name = AppPermissions.CreateWeather,
                    Roles = new[] { "Admin" }
                },
                new()
                {
                    Name = AppPermissions.DeleteWeather,
                    Roles = new[] { "Admin" }
                }
            });

        public Task<Policy> ReadPolicyAsync()
        {
            return Task.FromResult(_policy);
        }
    }
}
