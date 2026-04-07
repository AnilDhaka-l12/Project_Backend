using Amazon.S3;
using Amazon.S3.Model;
using ProjectBackend.Config.Backblaze;
using ProjectBackend.Model.Dto;
using ProjectBackend.Services.IServices;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace ProjectBackend.Services
{
    public class FileDownloadService : IFileDownloadService
    {
        private readonly IAmazonS3 _s3Client;
        private readonly string _bucketName;
        private readonly ILogger<FileDownloadService> _logger;

        public FileDownloadService(IConfiguration configuration, ILogger<FileDownloadService> logger)
        {
            _logger = logger;
            _s3Client = BackblazeInitializer.InitializeS3Client(configuration);
            _bucketName = BackblazeInitializer.GetBucketName(configuration);
        }

        public async Task<string> GenerateSecureDownloadUrlAsync(string fileKey, int expiryMinutes = 5)
        {
            if (!await FileExistsAsync(fileKey))
                throw new FileNotFoundException($"File {fileKey} not found");

            var request = new GetPreSignedUrlRequest
            {
                BucketName = _bucketName,
                Key = fileKey,
                Expires = DateTime.UtcNow.AddMinutes(expiryMinutes),
                Protocol = Protocol.HTTPS,
                Verb = HttpVerb.GET
            };

            return _s3Client.GetPreSignedURL(request);
        }

        public async Task<FileDownloadResponseDto> GetSecureDownloadLinkAsync(string fileKey, int expiryMinutes = 5)
        {
            var metadata = await GetFileMetadataAsync(fileKey);
            if (metadata == null)
                throw new FileNotFoundException($"File {fileKey} not found");

            var downloadUrl = await GenerateSecureDownloadUrlAsync(fileKey, expiryMinutes);
            
            return new FileDownloadResponseDto
            {
                DownloadUrl = downloadUrl,
                ExpiresInMinutes = expiryMinutes,
                ExpiresAt = DateTime.UtcNow.AddMinutes(expiryMinutes),
                FileKey = fileKey,
                FileName = metadata.FileName
            };
        }

        public async Task<bool> FileExistsAsync(string fileKey)
        {
            try
            {
                var request = new GetObjectMetadataRequest
                {
                    BucketName = _bucketName,
                    Key = fileKey
                };
                await _s3Client.GetObjectMetadataAsync(request);
                return true;
            }
            catch (AmazonS3Exception ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return false;
            }
        }

        public async Task<FileInfoDto?> GetFileMetadataAsync(string fileKey)
        {
            try
            {
                var request = new GetObjectMetadataRequest
                {
                    BucketName = _bucketName,
                    Key = fileKey
                };
                var response = await _s3Client.GetObjectMetadataAsync(request);
                
                var fileName = fileKey;
                var underscoreIndex = fileKey.IndexOf('_');
                if (underscoreIndex > 0)
                    fileName = fileKey[(underscoreIndex + 1)..];

                return new FileInfoDto
                {
                    FileKey = fileKey,
                    FileName = fileName,
                    FileSize = response.ContentLength,
                    ContentType = response.Headers.ContentType,
                    LastModified = response.LastModified ?? DateTime.UtcNow
                };
            }
            catch (AmazonS3Exception ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }
        }
    }
}