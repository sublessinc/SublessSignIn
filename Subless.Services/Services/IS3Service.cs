using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Subless.Services.Services
{
    public interface IS3Service
    {
        Task UploadFileToBucket(string filePath, string bucketName);
        Task<Uri> UploadFormFileToBucket(IFormFile formFile, string bucketName);
        Task<bool> CanAccessS3(string bucketName);
        Task DeleteFromBucket(string location, string bucketName);
    }
}
