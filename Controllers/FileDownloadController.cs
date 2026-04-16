using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectBackend.Model.Dto;
using ProjectBackend.Services.IServices;

namespace ProjectBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
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
    }
}