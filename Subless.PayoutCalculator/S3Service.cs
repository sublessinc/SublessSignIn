using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Transfer;
using CsvHelper;
using CsvHelper.Configuration;

namespace Subless.Services
{
    public class S3Service : IFileStorageService
    {
        private readonly TransferUtility transferUtility;
        public S3Service(AWSCredentials credentials)
        {
            var config = new TransferUtilityConfig
            {
                ConcurrentServiceRequests = 10,
                MinSizeBeforePartUpload = 16 * 1024 * 1024,
                NumberOfUploadThreads = 10
            };

            var s3Client = new AmazonS3Client(credentials, Amazon.RegionEndpoint.USEast1);
            transferUtility = new TransferUtility(s3Client, config);
        }

        public void WritePaymentsToCloudFileStore(Dictionary<string, double> masterPayoutList)
        {
            var csv = GetPathToGeneratedCsv(masterPayoutList);
            transferUtility.Upload(csv, "sublesslocaldevbucket");
        }

        private string GetPathToGeneratedCsv(Dictionary<string, double> masterPayoutList)
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
