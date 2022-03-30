using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Subless.Models;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;

namespace SublessSignIn.AuthServices
{
    public class BearerAuth
    {
        private AuthSettings AuthSettings;
        public const string PartnerSchemeName = "Bearer";
        public IServiceCollection AddBearerAuthServices(IServiceCollection services, AuthSettings authSettings)
        {

            AuthSettings = authSettings;
            services.AddAuthentication(PartnerSchemeName)
                 .AddJwtBearer(options =>
                 {
                     options.TokenValidationParameters = GetCognitoTokenValidationParams(AuthSettings);
                 });
            services.AddAuthorization(options => options.AddPolicy(PartnerSchemeName, policy =>
            {
                policy.AuthenticationSchemes.Add(PartnerSchemeName);
                policy.RequireClaim("client_id");
            }));
            return services;
        }

        private TokenValidationParameters GetCognitoTokenValidationParams(AuthSettings AuthSettings)
        {
            return new TokenValidationParameters
            {
                IssuerSigningKeyResolver = (s, securityToken, identifier, parameters) =>
                {
                    // get JsonWebKeySet from AWS
                    var json = new WebClient().DownloadString(AuthSettings.JwtKeySetUrl);

                    // serialize the result
                    var keys = JsonConvert.DeserializeObject<JsonWebKeySet>(json).Keys;

                    // cast the result to be the type expected by IssuerSigningKeyResolver
                    return keys;
                },
                ValidIssuer = AuthSettings.CognitoUrl,
                ValidateIssuerSigningKey = true,
                ValidateIssuer = true,
                ValidateLifetime = true,
                AudienceValidator = AudienceValidator,
            };
        }

        private bool AudienceValidator(IEnumerable<string> audiences, SecurityToken securityToken, TokenValidationParameters validationParameters)
        {
            if (securityToken is JwtSecurityToken)
            {
                var token = (JwtSecurityToken)securityToken;
                var tokenUse = token.Claims.FirstOrDefault(x => x.Type == "token_use")?.Value;
                if (tokenUse == null)
                {
                    return false;
                }
                if (tokenUse == "access")
                {
                    return true;
                }
                if (tokenUse == "id")
                {
                    return AuthSettings.AppClientId == audiences.First();
                }
            }
            return false;
        }
    }
}
