using ProjectBackend.Model.Dto;
using ProjectBackend.Services.IServices;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ProjectBackend.Helper;

namespace ProjectBackend.Services
{
    public class FileDownloadService : IFileDownloadService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<FileDownloadService> _logger;
        public FileDownloadService(IConfiguration configuration, ILogger<FileDownloadService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<FileDownloadResponseDto> GetSecureDownloadLinkAsync(string fileKey, int expiryMinutes = 5)
        {

            return new FileDownloadResponseDto
            {
                DownloadUrl = GitHubUrlHelper.GetDownloadUrl(_configuration, fileKey),
                ExpiresInMinutes = expiryMinutes,
                ExpiresAt = DateTime.UtcNow.AddMinutes(expiryMinutes),
                FileKey = fileKey,
                FileName = fileKey
            };
        }
    }
}
