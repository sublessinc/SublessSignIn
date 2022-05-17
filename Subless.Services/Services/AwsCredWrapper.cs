using Amazon.Runtime;
using Microsoft.Extensions.Options;

namespace Subless.Services.Services
{
    public class AwsCredWrapper
    {
        public AwsCredWrapper()
        {
        }

        public AWSCredentials GetCredentials()
        {
            return FallbackCredentialsFactory.GetCredentials();
        }
    }
}
