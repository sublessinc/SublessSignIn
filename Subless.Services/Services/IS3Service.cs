using System.Collections.Generic;

namespace Subless.Services
{
    public interface IS3Service
    {
        void WritePaymentsToS3(Dictionary<string, double> masterPayoutList);
    }
}