using Amazon.Runtime;
using Microsoft.Extensions.Options;
using Subless.PayoutCalculator;

namespace Subless.Services.Extensions
{
    public class AwsCredWrapper
    {
        public AwsCredWrapper(IOptions<CalculatorConfiguration> creds)
        {
        }

        public AWSCredentials GetCredentials()
        {
            return FallbackCredentialsFactory.GetCredentials();
        }
    }
}
