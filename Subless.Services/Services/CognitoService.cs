using Amazon;
using Amazon.CognitoIdentityProvider;
using Amazon.CognitoIdentityProvider.Model;
using Duende.Bff;
using Microsoft.Extensions.Options;
using Subless.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Subless.Services.Services
{
    public class CognitoService : ICognitoService, IDisposable
    {
        private readonly IUserSessionStore _userSessionStore;
        private readonly IOptions<DomainConfig> options;

        private readonly AmazonCognitoIdentityProviderClient _client;
        private readonly string PoolId;
        public CognitoService(IUserSessionStore userSessionStore ,IOptions<DomainConfig> options)
        {
            _userSessionStore = userSessionStore ?? throw new ArgumentNullException(nameof(userSessionStore));
            this.options = options ?? throw new ArgumentNullException(nameof(options));
            PoolId = options.Value?.UserPool ?? throw new ArgumentNullException(nameof(options.Value.UserPool));
            var region = RegionEndpoint.GetBySystemName(options.Value.Region);
            _client = new AmazonCognitoIdentityProviderClient(region: region);
        }

        public async Task DeleteCognitoUser(string cognitoUserId)
        {
            var deleteRequest = new AdminDeleteUserRequest
            {
                Username = cognitoUserId,
                UserPoolId = PoolId
            };
            await _client.AdminDeleteUserAsync(deleteRequest);

            await _userSessionStore.DeleteUserSessionsAsync(new UserSessionsFilter()
            {
                SubjectId = cognitoUserId,
            });            
        }

        public async Task<string> GetCognitoUserEmail(string cognitoUserId)
        {
            var user = await _client.AdminGetUserAsync(new AdminGetUserRequest()
            {
                Username = cognitoUserId,
                UserPoolId = PoolId
            });
            return user.UserAttributes.Single(x=>x.Name == "email").Value;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                // Dispose managed resources.
                _client.Dispose();
            }
            // Free native resources.
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
