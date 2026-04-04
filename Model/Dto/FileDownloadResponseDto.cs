namespace ProjectBackend.Model.Dto
{
    public class FileDownloadResponseDto
    {
        public string DownloadUrl { get; set; } = string.Empty;
        public int ExpiresInMinutes { get; set; }
        public DateTime ExpiresAt { get; set; }
        public string FileKey { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;
    }

    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public T? Data { get; set; }
        public string? Error { get; set; }
    }
}
