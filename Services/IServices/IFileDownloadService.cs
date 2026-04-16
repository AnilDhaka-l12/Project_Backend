using ProjectBackend.Model.Dto;

namespace ProjectBackend.Services.IServices
{
    public interface IFileDownloadService
    {

        /// <summary>
        /// Gets full download response with URL and metadata
        /// </summary>
        Task<FileDownloadResponseDto> GetSecureDownloadLinkAsync(string fileKey, int expiryMinutes = 5);
    }
}
