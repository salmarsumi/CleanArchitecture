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
                    Scopes = { "api.read", "api.write" }
                },
                new ApiResource("authorization", "Authorization Service")
                {
                    Scopes = { "authorization.read", "authorization.write" }
                }
            };

        public static IEnumerable<ApiScope> Scopes() =>
            new ApiScope[]
            {
                // API scopes
                new ApiScope(name: "api.read", displayName: "Reads data from api service."),
                new ApiScope(name: "api.write", displayName: "Writes data to the api service."),

                // authorization API scopes
                new ApiScope(name: "authorization.read", displayName: "Reads data from authorization service."),
                new ApiScope(name: "authorization.write", displayName: "Writes data to the authorization service.")
            };

        public static IEnumerable<Client> Clients() =>
            new Client[]
            {
                new Client
                {
                    ClientId = "angular",
                    ClientName = "Angular Web App",
                    ClientUri = "https://localhost:44427",
                    AllowedGrantTypes = GrantTypes.CodeAndClientCredentials,
                    RequireConsent = false,
                    RequirePkce = true,
                    AllowOfflineAccess = true,
                    AllowAccessTokensViaBrowser = false,

                    RedirectUris = { "https://localhost:44427/signin-oidc" },
                    PostLogoutRedirectUris = { "https://localhost:44427/signout-callback-oidc" },

                    ClientSecrets =
                    {
                        new Secret("b4ced783-dea4-4dd9-84eb-12a2f876bb51".Sha256())
                    },

                    AllowedScopes =
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.Email,
                        "api.read",
                        "api.write",
                        "authorization.read",
                        "authorization.write"
                    }
                }
            };
    }
}
