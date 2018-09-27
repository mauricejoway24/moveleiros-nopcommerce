using Microsoft.Owin.Security.OAuth;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Nop.Plugin.Api.OAuth.Providers
{
    public class AuthorizationBearerServerProvider : OAuthAuthorizationServerProvider
    {
        public override Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            var user = context.Parameters.Get("username");
            var pass = context.Parameters.Get("password");

           if (user != "NRF123@##Ad" || pass != "ASD1##a ASFh!# !#!#@ 454den (*7")
            {
                context.Rejected();
                return Task.FromResult(0);
            }

            context.Validated();

            return Task.FromResult(0);
        }

        public override Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            context.Validated(new ClaimsIdentity(context.Options.AuthenticationType));

            return base.GrantResourceOwnerCredentials(context);
        }
    }
}
