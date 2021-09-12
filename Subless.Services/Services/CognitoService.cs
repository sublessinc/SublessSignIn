using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Amazon.CognitoIdentityProvider;
using Amazon.CognitoIdentityProvider.Model;
using Amazon.IdentityManagement.Model;
using Microsoft.Extensions.Options;
using Subless.Models;

namespace Subless.Services.Services
{
    public class CognitoService : ICognitoService
    {
        private readonly IOptions<AuthSettings> options;

        private readonly AmazonCognitoIdentityProviderClient _client = new AmazonCognitoIdentityProviderClient();
        public CognitoService(IOptions<AuthSettings> options)
        {
            this.options = options ?? throw new ArgumentNullException(nameof(options));
            var poolId = options.Value?.PoolId ?? throw new ArgumentNullException(nameof(options.Value.PoolId));
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
