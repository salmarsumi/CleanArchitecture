using Duende.IdentityServer;
using Duende.IdentityServer.Models;

namespace CA.Identity
{
    public static class Config
    {
        private const string ANGULAR_CLIENT = "angular_client";

        // Identity resources are data like user ID, name, or email address of a user
        public static IEnumerable<IdentityResource> Resources() =>
            new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
                new IdentityResources.Email(),
            };

        public static IEnumerable<ApiResource> Apis() =>
            new ApiResource[]
            {
                new ApiResource("api", "Api Service")
                {
                    Scopes = { "api.full" },
                    UserClaims = { "name", "email" }
                },
                new ApiResource("authz", "Authorization Service")
                {
                    Scopes = { "authz.full" },
                    UserClaims = { "name", "email" }
                }
            };

        public static IEnumerable<ApiScope> Scopes() =>
            new ApiScope[]
            {
                // API scopes
                new ApiScope(name: "api.full", displayName: "Reads data from api service."),

                // authorization API scopes
                new ApiScope(name: "authz.full", displayName: "Reads data from authorization service."),
            };

        public static IEnumerable<Client> Clients() =>
            new Client[]
            {
                new Client
                {
                    ClientId = "angular",
                    ClientName = "Angular Web App",
                    ClientUri = "https://localhost:44480",
                    AllowedGrantTypes = GrantTypes.CodeAndClientCredentials,
                    RequireConsent = false,
                    RequirePkce = true,
                    // AllowOfflineAccess = true,
                    AllowAccessTokensViaBrowser = false,

                    RedirectUris = { "https://localhost:44480/signin-oidc" },
                    PostLogoutRedirectUris = { "https://localhost:44480/signout-callback-oidc" },

                    ClientSecrets =
                    {
                        new Secret("secret".Sha256())
                    },

                    AllowedScopes =
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.Email,
                        "api.full",
                        "authz.full"
                    }
                }
            };
    }
}
