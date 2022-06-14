using Amazon.S3;
using Amazon.S3.Transfer;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.Extensions.Options;
using Subless.Models;
using Subless.PayoutCalculator;
using Subless.Services.Extensions;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Subless.Services.Services
{
    public class S3Service : IFileStorageService, IDisposable
    {
        private readonly TransferUtility transferUtility;
        private readonly string BucketName;
        public S3Service(AwsCredWrapper credentials, IOptions<CalculatorConfiguration> options)
        {
            var config = new TransferUtilityConfig
            {
                ConcurrentServiceRequests = 10,
                MinSizeBeforePartUpload = 16 * 1024 * 1024
            };

            var s3Client = new AmazonS3Client(credentials.GetCredentials(), Amazon.RegionEndpoint.USEast1);
            transferUtility = new TransferUtility(s3Client, config);
            BucketName = options?.Value?.BucketName ?? throw new ArgumentNullException(nameof(options));
        }

        public void WritePaymentsToCloudFileStore(List<PaymentAuditLog> masterPayoutList)
        {
            var payoutsInPaypalFormat = masterPayoutList.Select(x => new PayPalItem()
            {
                Email = x.PayPalId,
                Amount = x.Payment
            }).ToList();
            var csv = GetPathToGeneratedCsv(payoutsInPaypalFormat);
            transferUtility.Upload(csv, BucketName);
        }

        /// <summary>
        /// Just checks if S3 is accessible
        /// </summary>
        /// <returns></returns>
        public async Task<bool> CanAccessS3()
        {
            var info = await transferUtility.S3Client.GetBucketLocationAsync(BucketName);
            if (info != null)
            {
                return true;
            }
            return false;
        }

        private string GetPathToGeneratedCsv(List<PayPalItem> masterPayoutList)
        {
            var filePath = Path.Join(Path.GetTempPath(), DateTimeOffset.UtcNow.ToString("yyyy-MM-dd-HH-mm-ss", CultureInfo.InvariantCulture) + ".csv");
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

        public void Dispose()
        {
            transferUtility.Dispose();
        }
    }
}
