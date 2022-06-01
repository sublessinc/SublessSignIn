using Amazon.Runtime;
using Microsoft.Extensions.Options;

namespace Subless.Services.Services
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
