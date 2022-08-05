using Duende.IdentityServer;
using Duende.IdentityServer.Models;

namespace CA.Identity
{
    public static class Config
    {
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
                },
                new ApiResource("audit", "Audit Service")
                {
                    Scopes = { "audit.full" },
                    UserClaims= { "name", "email" }
                }
            };

        public static IEnumerable<ApiScope> Scopes() =>
            new ApiScope[]
            {
                // API scopes
                new ApiScope(name: "api.full", displayName: "Reads data from api service."),

                // authorization API scopes
                new ApiScope(name: "authz.full", displayName: "Reads data from authorization service."),

                // audit Api scopes
                new ApiScope(name: "audit.full", displayName: "Reads data from audit service" ),
            };

        public static IEnumerable<Client> Clients(string clientUri) =>
            new Client[]
            {
                new Client
                {
                    ClientId = "angular",
                    ClientName = "Angular Web App",
                    ClientUri = clientUri,
                    AllowedGrantTypes = GrantTypes.CodeAndClientCredentials,
                    RequireConsent = false,
                    RequirePkce = true,
                    AllowAccessTokensViaBrowser = false,

                    RedirectUris = { $"{clientUri}/signin-oidc" },
                    PostLogoutRedirectUris = { $"{clientUri}/signout-callback-oidc" },

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
                        "authz.full",
                        "audit.full",
                    }
                }
            };
    }
}
