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

        public async Task<FileDownloadResponseDto> GetSecureDownloadLinkAsync(string fileKey, string platform, int expiryMinutes = 5)
        {
            var downloadUrl = GitHubUrlHelper.GetDownloadUrl(_configuration, fileKey, platform);

            // Set proper filename based on platform
            var fileName = platform?.ToLower() == "windows"
                ? "mysetup.exe"
                : "jupyter_manager_linux_amd64.tar.gz";

            return new FileDownloadResponseDto
            {
                DownloadUrl = downloadUrl,
                ExpiresInMinutes = expiryMinutes,
                ExpiresAt = DateTime.UtcNow.AddMinutes(expiryMinutes),
                FileKey = fileKey,
                FileName = fileName
            };
        }
    }
}
