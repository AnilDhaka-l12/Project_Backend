// Model/FileLuSettings.cs
namespace ProjectBackend.Model
{
    public class FileLuSettings
    {
        public string AccessKey { get; set; } = string.Empty;
        public string SecretKey { get; set; } = string.Empty;
        public string EndpointUrl { get; set; } = "https://s5lu.com";
        public string BucketName { get; set; } = string.Empty;
        public int DefaultExpiryMinutes { get; set; } = 5;
    }
}
