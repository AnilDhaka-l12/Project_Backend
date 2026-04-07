using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectBackend.Model.Dto;
using ProjectBackend.Services.IServices;

namespace ProjectBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // keep this if middleware handles auth
    public class FileDownloadController : ControllerBase
    {
        private readonly IFileDownloadService _fileDownloadService;
        private readonly ILogger<FileDownloadController> _logger;

        public FileDownloadController(
            IFileDownloadService fileDownloadService,
            ILogger<FileDownloadController> logger)
        {
            _fileDownloadService = fileDownloadService;
            _logger = logger;
        }

        /// <summary>
        /// Get a secure download link for a file
        /// </summary>
        [HttpGet("link/{fileKey}")]
        public async Task<ActionResult<ApiResponse<FileDownloadResponseDto>>> GetDownloadLink(
            string fileKey,
            [FromQuery] int expiryMinutes = 5)
        {
            try
            {
                expiryMinutes = Math.Clamp(expiryMinutes, 1, 60);

                var result = await _fileDownloadService.GetSecureDownloadLinkAsync(fileKey, expiryMinutes);

                _logger.LogInformation(
                    "Download link generated for {FileKey}, expires in {ExpiryMinutes} minutes",
                    fileKey, expiryMinutes);

                return Ok(new ApiResponse<FileDownloadResponseDto>
                {
                    Success = true,
                    Message = "Download link generated successfully",
                    Data = result
                });
            }
            catch (FileNotFoundException)
            {
                return NotFound(new ApiResponse<FileDownloadResponseDto>
                {
                    Success = false,
                    Message = "File not found",
                    Error = $"File with key {fileKey} does not exist"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating download link for {FileKey}", fileKey);
                return StatusCode(500, new ApiResponse<FileDownloadResponseDto>
                {
                    Success = false,
                    Message = "An error occurred while generating download link",
                    Error = ex.Message
                });
            }
        }

        /// <summary>
        /// Check if a file exists
        /// </summary>
        [HttpGet("exists/{fileKey}")]
        public async Task<ActionResult<ApiResponse<bool>>> CheckFileExists(string fileKey)
        {
            try
            {
                var exists = await _fileDownloadService.FileExistsAsync(fileKey);

                return Ok(new ApiResponse<bool>
                {
                    Success = true,
                    Message = exists ? "File exists" : "File not found",
                    Data = exists
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking file existence for {FileKey}", fileKey);
                return StatusCode(500, new ApiResponse<bool>
                {
                    Success = false,
                    Message = "An error occurred",
                    Error = ex.Message
                });
            }
        }

        /// <summary>
        /// Get file metadata
        /// </summary>
        [HttpGet("info/{fileKey}")]
        public async Task<ActionResult<ApiResponse<FileInfoDto>>> GetFileInfo(string fileKey)
        {
            try
            {
                var metadata = await _fileDownloadService.GetFileMetadataAsync(fileKey);

                if (metadata == null)
                {
                    return NotFound(new ApiResponse<FileInfoDto>
                    {
                        Success = false,
                        Message = "File not found",
                        Error = $"File with key {fileKey} does not exist"
                    });
                }

                return Ok(new ApiResponse<FileInfoDto>
                {
                    Success = true,
                    Message = "File metadata retrieved successfully",
                    Data = metadata
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting file info for {FileKey}", fileKey);
                return StatusCode(500, new ApiResponse<FileInfoDto>
                {
                    Success = false,
                    Message = "An error occurred",
                    Error = ex.Message
                });
            }
        }
    }
}