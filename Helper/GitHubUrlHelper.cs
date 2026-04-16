using Microsoft.Extensions.Configuration;

namespace ProjectBackend.Helper;

public static class GitHubUrlHelper
{
    private static readonly IConfiguration _config;
    
    static GitHubUrlHelper()
    {
        _config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .Build();
    }
    
    public static string GetDownloadUrl(string filename)
    {
        var owner = _config["GitHub:Owner"];
        var repo = _config["GitHub:Repo"];
        var baseUrl = _config["GitHub:BaseUrl"];
        var tag = _config["GitHub:DefaultTag"];
        
        return $"{baseUrl}/{owner}/{repo}/releases/{tag}/download/{filename}";
    }
}