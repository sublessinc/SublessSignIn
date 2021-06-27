using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Amazon.Runtime;
using Microsoft.Extensions.Options;

namespace Subless.PayoutCalculator
{
    public class AwsCredWrapper : BasicAWSCredentials
    {
        public AwsCredWrapper(IOptions<AwsCreds> creds) : base(creds.Value.AccessKey, creds.Value.SecretKey)
        {
        }
    }
}
