using System;
using System.Linq;
using System.Threading.Tasks;
using Amazon;
using Amazon.CognitoIdentityProvider;
using Amazon.CognitoIdentityProvider.Model;
using Duende.Bff;
using Microsoft.Extensions.Options;
using Subless.Models;

namespace Subless.Services.Services
{
    public class CognitoService : ICognitoService, IDisposable
    {
        private readonly IUserSessionStore _userSessionStore;
        private readonly IOptions<DomainConfig> options;

        private readonly AmazonCognitoIdentityProviderClient _client;
        private readonly string PoolId;
        public CognitoService(IUserSessionStore userSessionStore, IOptions<DomainConfig> options)
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

        public async Task<string?> GetCongitoUserByEmail(string email)
        {
            if (email is null)
            {
                throw new ArgumentNullException(nameof(email));
            }

            var response = await _client.ListUsersAsync(new ListUsersRequest()
            {
                Filter = $"email=\"{email}\"",
                UserPoolId = PoolId
            });
            if (response.Users.Any())
            {
                return response.Users.Single().Username;
            }
            return null;
        }

        public async Task<string?> GetCognitoUserEmail(string cognitoUserId)
        {
            try
            {
                var user = await _client.AdminGetUserAsync(new AdminGetUserRequest()
                {
                    Username = cognitoUserId,
                    UserPoolId = PoolId
                });
                if (user.UserAttributes.Any(x => x.Name == "email"))
                {
                    return user.UserAttributes.Single(x => x.Name == "email")?.Value;
                }
                return null;
            }
            catch (AggregateException e)
            {
                if (!e.InnerExceptions.Any(x => x is UserNotFoundException))
                {
                    throw;
                }
                return null;
            }
            catch (UserNotFoundException)
            {
                return null;
            }
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
