using IdentityServer4;
using IdentityServer4.Models;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

namespace IdentityServer
{
    public static class Config
    {
        //public static string HOST_URL = "https://localhost:44300";

        public static IEnumerable<IdentityResource> IdentityResources =>
            new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
            };


        public static IEnumerable<ApiScope> ApiScopes =>
            new List<ApiScope>
            {
                new ApiScope("api")
            };

        public static IEnumerable<Client> Clients(IConfiguration configuration) =>
            new List<Client>
            {                
                // interactive ASP.NET Core MVC client
                new Client
                {
                    ClientId = "mvc",
                    ClientSecrets = { new Secret("secret".Sha256()) },

                    AllowedGrantTypes = GrantTypes.Code,
                    AllowOfflineAccess = true,
                    
                    // where to redirect to after login
                    RedirectUris = { configuration["RedirectUri"]/*"https://localhost:44300/signin-oidc"*/ },

                    // where to redirect to after logout
                    PostLogoutRedirectUris = { configuration["PostLogoutRedirectUri"]/*"https://localhost:44300/signout-callback-oidc"*/ },

                    AllowedScopes = new List<string>
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        "api"

                    }
                }
            };
    }
}