using ProjectBackend.Model.Dto;

namespace ProjectBackend.Services.IServices
{
    public interface IFileDownloadService
    {

        /// <summary>
        /// Gets full download response with URL and metadata
        /// </summary>
        Task<FileDownloadResponseDto> GetSecureDownloadLinkAsync(string fileKey, string platfrom, int expiryMinutes = 5);
    }
}
