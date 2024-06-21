using CA.Common.Authorization.PolicyRuntime;
using Microsoft.Extensions.Caching.Distributed;
using System.Net;
using System.Security.Claims;
using System.Text.Json;

namespace CA.Common.Authorization.Client
{
    /// <summary>
    /// Provides an implementation to evaluate users through calling the Authorization Service.
    /// This class is will be used through the IPolicyOperations interface for remote evaluation operations.
    /// </summary>
    public class RemotePolicyOperations : IPolicyOperations
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IDistributedCache _cache;
        private PolicyEvaluationResult _userPolicy;

        public RemotePolicyOperations(IHttpClientFactory clientFactory,
            IDistributedCache cache)
        {
            _httpClientFactory = clientFactory;
            _cache = cache;
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
                        var client = _httpClientFactory
                            .CreateClient("authorizationApi") ??
                                throw new InvalidOperationException("Create clinet for name authorizationApi returned null");

                        var response = await client.GetAsync("/policy");
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
                            // the policy take effect within 10 seconds at the most
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

        public async Task<bool> IsInGroupAsync(ClaimsPrincipal user, string group)
        {
            var policy = await EvaluateAsync(user);
            return policy?.Groups?.Contains(group) ?? false;
        }
    }
}
