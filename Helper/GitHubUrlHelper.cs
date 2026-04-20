using Microsoft.Extensions.Configuration;
using System;

namespace ProjectBackend.Helper;

public static class GitHubUrlHelper
{
    public static string GetDownloadUrl(IConfiguration configuration, string version)
    {
        var owner = configuration["GitHub:Owner"]
            ?? throw new InvalidOperationException("GitHub:Owner not configured");

        var repo = configuration["GitHub:Repo"]
            ?? throw new InvalidOperationException("GitHub:Repo not configured");

        var tag = configuration["GitHub:DefaultTag"] ?? "latest";

        var fileName = configuration["GitHub:fileName"]
            ?? throw new InvalidOperationException("GitHub:fileName not configured");

        // Fix: For "latest" tag, use a different URL pattern
        if (tag == "latest")
        {
            return $"https://github.com/{owner}/{repo}/releases/latest/download/{version}";
        }
        
        return $"https://github.com/{owner}/{repo}/releases/download/{tag}/{version}/{fileName}";
    }
}