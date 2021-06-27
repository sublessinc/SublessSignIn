using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Transfer;
using CsvHelper;
using CsvHelper.Configuration;

namespace Subless.Services
{
    public class S3Service : IS3Service
    {
        private readonly TransferUtility transferUtility;
        public S3Service(AWSCredentials credentials)
        {
            var config = new TransferUtilityConfig();

            config.ConcurrentServiceRequests = 10;
            config.MinSizeBeforePartUpload = 16 * 1024 * 1024;
            config.NumberOfUploadThreads = 10;

            var s3Client = new AmazonS3Client(credentials);
            transferUtility = new TransferUtility(s3Client, config);
        }

        public void WritePaymentsToS3(Dictionary<string, double> masterPayoutList)
        {
            var csv = GetCsv(masterPayoutList);
            transferUtility.Upload(csv, "sublesslocaldevbucket");
        }

        private string GetCsv(Dictionary<string, double> masterPayoutList )
        {
            var filePath = Path.Join(Path.GetTempPath(), DateTime.UtcNow.ToOADate() + ".csv");
            using (var writer = new StreamWriter(filePath))
            using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                csv.Context.RegisterClassMap<DictionaryMap>();
                csv.WriteRecords(masterPayoutList);
            }
            return filePath;
        }
        public class DictionaryMap : ClassMap<KeyValuePair<string, double>>
        {
            public DictionaryMap()
            {
                Map(m => m.Key).Index(0).Name("PayoneerId");
                Map(m => m.Value).Index(1).Name("PaymentDollars");
            }
        }
    }
}
