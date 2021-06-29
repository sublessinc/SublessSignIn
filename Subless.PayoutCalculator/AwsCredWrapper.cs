using Amazon.Runtime;
using Microsoft.Extensions.Options;

namespace Subless.PayoutCalculator
{
    public class AwsCredWrapper : BasicAWSCredentials
    {
        public AwsCredWrapper(IOptions<AwsConfiguration> creds) : base(creds.Value.AccessKey, creds.Value.SecretKey)
        {
        }
    }
}
