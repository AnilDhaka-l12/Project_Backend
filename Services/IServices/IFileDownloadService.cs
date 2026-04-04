using ProjectBackend.Model.Dto;

namespace ProjectBackend.Services.IServices
{
    public interface IFileDownloadService
    {
        /// <summary>
        /// Generates a secure, time-limited pre-signed URL for downloading
        /// </summary>
        Task<string> GenerateSecureDownloadUrlAsync(string fileKey, int expiryMinutes = 5);

        /// <summary>
        /// Gets full download response with URL and metadata
        /// </summary>
        Task<FileDownloadResponseDto> GetSecureDownloadLinkAsync(string fileKey, int expiryMinutes = 5);

        /// <summary>
        /// Checks if a file exists in storage
        /// </summary>
        Task<bool> FileExistsAsync(string fileKey);

        /// <summary>
        /// Gets file metadata (size, last modified, etc.)
        /// </summary>
        Task<FileInfoDto?> GetFileMetadataAsync(string fileKey);
    }
}
