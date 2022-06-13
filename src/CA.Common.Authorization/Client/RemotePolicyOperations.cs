using CA.Common.Authorization.PolicyRuntime;
using Microsoft.Extensions.Caching.Distributed;
using System.Net;
using System.Security.Claims;
using System.Text.Json;

namespace CA.Common.Authorization.Client
{
    public class RemotePolicyOperations : IPolicyOperations
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly IDistributedCache _cache;
        private readonly HttpClient _client;
        private PolicyEvaluationResult _userPolicy;

        public RemotePolicyOperations(IHttpClientFactory clientFactory,
            IDistributedCache cache)
        {
            _clientFactory = clientFactory;
            _cache = cache;

            _client = _clientFactory
                .CreateClient("authorizationApi") ?? 
                throw new InvalidOperationException("Create clinet for name authorizationApi returned null");
        }

        public async Task<PolicyEvaluationResult> EvaluateAsync(ClaimsPrincipal user)
        {
            if (user is null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            if (_userPolicy is null)
            {
                // check for a cached copy of the user permissions
                var sub = user.FindFirst("sub")?.Value;
                if (!string.IsNullOrEmpty(sub))
                {
                    var jsonOptions = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };

                    var result = await _cache.GetStringAsync($"policy-{sub}");
                    if (!string.IsNullOrEmpty(result))
                        _userPolicy = JsonSerializer.Deserialize<PolicyEvaluationResult>(result, jsonOptions);

                    // if no cached copy is found call the authorization service and cache the result for a short period
                    if (_userPolicy is null)
                    {
                        var serialized = string.Empty;
                        var response = await _client.GetAsync("/policy");
                        if(response.StatusCode == HttpStatusCode.OK)
                        {
                            serialized = await response.Content.ReadAsStringAsync();
                            _userPolicy = JsonSerializer.Deserialize<PolicyEvaluationResult>(serialized, jsonOptions);
                        }

                        if (_userPolicy != null)
                        {
                            // cache for 10 seconds total with a sliding timer of 5 seconds
                            // this will cache the policy at the remote client for a short time
                            // avoiding multiple consecutive server request while making changes
                            // the policy take effect withen 10 seconds at the most
                            await _cache.SetStringAsync($"policy-{sub}", serialized, new DistributedCacheEntryOptions
                            {
                                AbsoluteExpiration = DateTime.Now.AddSeconds(10),
                                SlidingExpiration = TimeSpan.FromSeconds(5)
                            });
                        }
                    }
                }
            }

            return _userPolicy;
        }

        public async Task<bool> HasPermissionAsync(ClaimsPrincipal user, string permission)
        {
            var policy = await EvaluateAsync(user);
            return policy?.Permissions?.Contains(permission) ?? false;
        }

        public async Task<bool> IsInRoleAsync(ClaimsPrincipal user, string role)
        {
            var policy = await EvaluateAsync(user);
            return policy?.Roles?.Contains(role) ?? false;
        }
    }
}
