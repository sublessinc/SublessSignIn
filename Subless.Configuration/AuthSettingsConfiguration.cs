using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Subless.Models;
using static Subless.Data.DataDi;

namespace Subless.Services
{
    public static class AuthSettingsConfiguration
    {
        public static AuthSettings GetAuthSettings()
        {
            var authSettings = new AuthSettings
            {
                Region = Environment.GetEnvironmentVariable("region") ?? throw new ArgumentNullException("region"),
                PoolId = Environment.GetEnvironmentVariable("userPoolId") ?? throw new ArgumentNullException("userPoolId"),
                AppClientId = Environment.GetEnvironmentVariable("appClientId") ?? throw new ArgumentNullException("appClientId")
            };
            authSettings.IssuerUrl = Environment.GetEnvironmentVariable("issuerUrl") ?? throw new ArgumentNullException("issuerUrl");
            authSettings.CognitoUrl = $"https://cognito-idp.{authSettings.Region}.amazonaws.com/{authSettings.PoolId}";
            authSettings.JwtKeySetUrl = authSettings.CognitoUrl + "/.well-known/jwks.json";
            authSettings.Domain = Environment.GetEnvironmentVariable("DOMAIN") ?? throw new ArgumentNullException("DOMAIN");
            authSettings.IdentityServerLicenseKey = Environment.GetEnvironmentVariable("IdentityServerLicenseKey") ?? "";
            var json = Environment.GetEnvironmentVariable("dbCreds") ?? throw new ArgumentNullException("dbCreds");
            var dbCreds = JsonConvert.DeserializeObject<DbCreds>(json);
            authSettings.SessionStoreConnString = dbCreds.GetDatabaseConnection();
            if (!authSettings.Domain.EndsWith('/'))
            {
                authSettings.Domain += '/';
            }
            return authSettings;
        }

        public static IServiceCollection RegisterAuthSettingsConfig(IServiceCollection services, AuthSettings authSettings)
        {
            services.Configure<AuthSettings>(options =>
            {
                options.AppClientId = authSettings.AppClientId;
                options.CognitoUrl = authSettings.CognitoUrl;
                options.IssuerUrl = authSettings.IssuerUrl;
                options.JwtKeySetUrl = authSettings.JwtKeySetUrl;
                options.PoolId = authSettings.PoolId;
                options.Region = authSettings.Region;
                options.Domain = authSettings.Domain;
                options.IdentityServerLicenseKey = authSettings.IdentityServerLicenseKey;
                options.SessionStoreConnString = authSettings.SessionStoreConnString;
            });
            return services;
        }
    }
}
