using Amazon.Runtime;
using Microsoft.Extensions.Options;

namespace Subless.PayoutCalculator
{
    public class AwsCredWrapper
    {
        public AwsCredWrapper(IOptions<AwsConfiguration> creds)
        {
        }

        public AWSCredentials GetCredentials()
        {
            return FallbackCredentialsFactory.GetCredentials();
        }
    }
}
