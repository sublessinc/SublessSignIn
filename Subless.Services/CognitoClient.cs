using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using Amazon;
using Amazon.CognitoIdentity;
using Amazon.CognitoIdentityProvider;
using Amazon.Extensions.CognitoAuthentication;
using Amazon.Runtime;

namespace Subless.Services
{
    public class CognitoClient
    {
        public async void GetLoginEvents(string username)
        {

            var credentials = new BasicAWSCredentials("dicks", "ball");
            var provider = new AmazonCognitoIdentityProviderClient(credentials);
            var request = new Amazon.CognitoIdentityProvider.Model.AdminListUserAuthEventsRequest()
            {
                MaxResults = 60,
                Username = username,
                //UserPoolId =
                //appClientId = 4a8epi5e1hh6bp8cr761i24um5
                UserPoolId = "us-east-1_6U7tGA4YG"
            };
            var results = await provider.AdminListUserAuthEventsAsync(request);
            var auths = results.AuthEvents.ToList();
            var auth = auths.First();
            //awsCredentials = internalAwsCredentials;
            //var adminAmazonCognitoIdentityProviderClient = new AmazonCognitoIdentityProviderClient(
            //    awsCredentials,
            //    regionEndpoint);
            //var anonymousAmazonCognitoIdentityProviderClient = new AmazonCognitoIdentityProviderClient(
            //    new AnonymousAWSCredentials(),
            //    regionEndpoint);
            //CognitoUserPool userPool = new CognitoUserPool("poolID", "clientID", provider);
            //CognitoUser user = new CognitoUser("username", "clientID", userPool, provider);
            //user.Get
            //string password = "userPassword";

            //AuthFlowResponse context = await user.StartWithSrpAuthAsync(new InitiateSrpAuthRequest()
            //{
            //    Password = password
            //}).ConfigureAwait(false);

            //CognitoAWSCredentials credentials =
            //    user.GetCognitoAWSCredentials("identityPoolID", RegionEndpoint.< YourIdentityPoolRegion >);

            //using (var client = new AmazonS3Client(credentials))
            //{
            //    ListBucketsResponse response =
            //        await client.ListBucketsAsync(new ListBucketsRequest()).ConfigureAwait(false);

            //    foreach (S3Bucket bucket in response.Buckets)
            //    {
            //        Console.WriteLine(bucket.BucketName);
            //    }
            //}
        }
    }
}
