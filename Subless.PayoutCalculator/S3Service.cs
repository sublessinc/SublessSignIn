using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
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
                MinSizeBeforePartUpload = 16 * 1024 * 1024
            };

            var s3Client = new AmazonS3Client(credentials, Amazon.RegionEndpoint.USEast1);
            transferUtility = new TransferUtility(s3Client, config);
        }

        public void WritePaymentsToCloudFileStore(Dictionary<string, double> masterPayoutList)
        {
            var payoutsInPaypalFormat = masterPayoutList.Select(x=> new PayPalItem()
            {
                Email = x.Key,
                Amount = x.Value                
            }).ToList();
            var csv = GetPathToGeneratedCsv(payoutsInPaypalFormat);
            transferUtility.Upload(csv, "sublesslocaldevbucket");
        }

        private string GetPathToGeneratedCsv(List<PayPalItem> masterPayoutList)
        {
            var filePath = Path.Join(Path.GetTempPath(), DateTime.UtcNow.ToString("yyyy-MM-dd-HH-mm-ss") + ".csv");
            using (var writer = new StreamWriter(filePath))
            using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                csv.Context.RegisterClassMap<ItemToCsvMap>();
                csv.WriteRecords(masterPayoutList);
            }
            return filePath;
        }
        public class ItemToCsvMap : ClassMap<PayPalItem>
        {
            public ItemToCsvMap()
            {
                Map(m => m.Email).Index(0).Name("Email/Phone");
                Map(m => m.Amount).Index(1).Name("Amount");
                Map(m => m.Currency).Index(2).Name("Currency code");
                Map(m => m.Reference).Index(3).Name("Reference ID (optional)");
                Map(m => m.Note).Index(4).Name("Note to recipient");
                Map(m => m.Wallet).Index(5).Name("Recipient wallet");
                Map(m => m.Social).Index(6).Name("Social Feed Privacy (optional)");
                Map(m => m.Holler).Index(7).Name("Holler URL (optional)");
                Map(m => m.Logo).Index(8).Name("Logo URL (optional)");
            }
        }

        public class PayPalItem
        {
            public string Email { get; set; }
            public double Amount { get; set; }
            public string Currency => "USD";
            public string Reference { get; set; }
            public string Note => "Your subless payout for the month";
            public string Wallet { get; set; }
            public string Social { get; set; }
            public string Holler { get; set; }
            public string Logo => "https://pay.subless.com/assets/img/SublessLogoOnly.png";
        }
    }
}
