using Duende.IdentityModel;
using Duende.IdentityServer;
using Duende.IdentityServer.Models;

public static class Config
{
    public static IEnumerable<IdentityResource> IdentityResources =>
        new IdentityResource[]
        {
            new IdentityResources.OpenId(),
            new IdentityResources.Profile(),
             new IdentityResource(
                "roles",
                new[] { JwtClaimTypes.Role }
            )
        };

    public static IEnumerable<ApiScope> ApiScopes =>
        new ApiScope[]
        {
            new ApiScope("airsoftevents.api.read"),
            new ApiScope("airsoftevents.api.write"),
            new ApiScope("airsoftevents.api.admin"),
        };

    public static IEnumerable<Client> Clients =>
        new Client[]
        {
            new Client
            {
                ClientId = "postman-client",
                AllowedGrantTypes = GrantTypes.ClientCredentials,
                ClientSecrets = { new Secret("eenGrootGeheim".Sha256()) },
                AllowedScopes = { "airsoftevents.api.read","airsoftevents.api.write", "airsoftevents.api.admin"}
            },

            
             new Client
            {
                ClientId = "webapp-client",
                AllowedGrantTypes = GrantTypes.Code,
                RequirePkce = true,
                RequireClientSecret = false, // SPA in dev

                RedirectUris = { "http://localhost:5173/" },
                PostLogoutRedirectUris = { "http://localhost:5173/" },
                AllowedCorsOrigins = { "http://localhost:5173" },

                AllowedScopes =
                {
                    IdentityServerConstants.StandardScopes.OpenId,
                    IdentityServerConstants.StandardScopes.Profile,
                    "roles",
                    "airsoftevents.api.read",
                    "airsoftevents.api.write",
                    "airsoftevents.api.admin",
                },

                AllowOfflineAccess = false
            }
        };
}
