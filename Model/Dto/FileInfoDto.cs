namespace ProjectBackend.Model.Dto
{
    public class FileInfoDto
    {
        public string FileKey { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;
        public long FileSize { get; set; }
        public string ContentType { get; set; } = string.Empty;
        public DateTime LastModified { get; set; }
    }
}
