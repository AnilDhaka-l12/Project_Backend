using Microsoft.Extensions.Configuration;
using System;

namespace ProjectBackend.Helper;

public static class GitHubUrlHelper
{
    public static string GetDownloadUrl(IConfiguration configuration, string filename)
    {
        var owner = configuration["GitHub:Owner"]
            ?? throw new InvalidOperationException("GitHub:Owner not configured");

        var repo = configuration["GitHub:Repo"]
            ?? throw new InvalidOperationException("GitHub:Repo not configured");

        var baseUrl = configuration["GitHub:BaseUrl"] ?? "https://github.com";
        var tag = configuration["GitHub:DefaultTag"] ?? "latest";

        return $"{baseUrl}/{owner}/{repo}/releases/{tag}/download/{filename}";
    }
}
