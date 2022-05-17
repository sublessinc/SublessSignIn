using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using Microsoft.AspNetCore.Http;

namespace Subless.Services.Services
{
    public class S3Service : IS3Service, IDisposable
    {
        private TransferUtility _transferUtility;
        private AmazonS3Client _s3Client;
        public S3Service(AwsCredWrapper credentials)
        {
            var config = new TransferUtilityConfig
            {
                ConcurrentServiceRequests = 10,
                MinSizeBeforePartUpload = 16 * 1024 * 1024
            };

            _s3Client = new AmazonS3Client(credentials.GetCredentials(), Amazon.RegionEndpoint.USEast1);
            _transferUtility = new TransferUtility(_s3Client, config);
        }

        public async Task<Uri> UploadFormFileToBucket(IFormFile formFile, string bucketName)
        {
            var location = $"{Guid.NewGuid}.{formFile.FileName}";
            using (var stream = formFile.OpenReadStream())
            {
                var putRequest = new PutObjectRequest
                {
                    Key = location,
                    BucketName = bucketName,
                    InputStream = stream,
                    AutoCloseStream = true,
                    ContentType = formFile.ContentType
                };
                var response = await _s3Client.PutObjectAsync(putRequest);
                var itemUrl = new Uri($"https://{bucketName}.s3.{Amazon.RegionEndpoint.USEast2}.amazonaws.com/{location}");
                return itemUrl;
            }
        }

        public async Task UploadFileToBucket(string filePath, string bucketName)
        {
            _transferUtility.Upload(filePath, bucketName);
        }

        public async Task DeleteFromBucket(string location, string bucketName)
        {
            var deleteRequest = new DeleteObjectRequest()
            {
                Key = location,
                BucketName = bucketName
            };
            await _s3Client.DeleteObjectAsync(deleteRequest);
        }

        /// <summary>
        /// Just checks if S3 is accessible
        /// </summary>
        /// <returns></returns>
        public async Task<bool> CanAccessS3(string bucketName)
        {
            var info = await _transferUtility.S3Client.GetBucketLocationAsync(bucketName);
            if (info != null)
            {
                return true;
            }
            return false;
        }

        public void Dispose()
        {
            _transferUtility.Dispose();
            _s3Client.Dispose();
        }
    }
}
