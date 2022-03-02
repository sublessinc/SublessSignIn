using Amazon;
using Amazon.CognitoIdentityProvider;
using Amazon.CognitoIdentityProvider.Model;
using Microsoft.Extensions.Options;
using Subless.Models;
using System;
using System.Threading.Tasks;

namespace Subless.Services.Services
{
    public class CognitoService : ICognitoService
    {
        private readonly IOptions<AuthSettings> options;

        private readonly AmazonCognitoIdentityProviderClient _client;
        public CognitoService(IOptions<AuthSettings> options)
        {
            this.options = options ?? throw new ArgumentNullException(nameof(options));
            var poolId = options.Value?.PoolId ?? throw new ArgumentNullException(nameof(options.Value.PoolId));
            var region = RegionEndpoint.GetBySystemName(options.Value.Region);
            _client = new AmazonCognitoIdentityProviderClient(region: region);
        }

        public async Task DeleteCognitoUser(string cognitoUserId)
        {
            var deleteRequest = new AdminDeleteUserRequest
            {
                Username = cognitoUserId,
                UserPoolId = options.Value.PoolId
            };
            await _client.AdminDeleteUserAsync(deleteRequest);
        }
    }
}
