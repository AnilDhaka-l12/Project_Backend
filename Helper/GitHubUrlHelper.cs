using Microsoft.Extensions.Configuration;
using System;

namespace ProjectBackend.Helper;

public static class GitHubUrlHelper
{
    public static string GetDownloadUrl(IConfiguration configuration, string version, string platform)
    {
        // Determine which config section to use based on platform
        var configSection = platform?.ToLower() == "windows" ? "GitHub" : "GitHubLinux";

        var owner = configuration[$"{configSection}:Owner"]
            ?? throw new InvalidOperationException($"{configSection}:Owner not configured");

        var repo = configuration[$"{configSection}:Repo"]
            ?? throw new InvalidOperationException($"{configSection}:Repo not configured");

        var tag = configuration[$"{configSection}:DefaultTag"] ?? "latest";

        var fileName = configuration[$"{configSection}:fileName"]
            ?? throw new InvalidOperationException($"{configSection}:fileName not configured");

        // For "latest" tag
        if (tag == "latest")
        {
            return $"https://github.com/{owner}/{repo}/releases/latest/download/{fileName}";
        }

        // For specific version/tag
        return $"https://github.com/{owner}/{repo}/releases/download/{tag}/{fileName}";
    }

    // Override for platform-specific calls
    public static string GetWindowsDownloadUrl(IConfiguration configuration, string version = null)
    {
        var tag = version ?? configuration["GitHub:DefaultTag"] ?? "latest";
        return GetDownloadUrl(configuration, tag, "windows");
    }

    public static string GetLinuxDownloadUrl(IConfiguration configuration, string version = null)
    {
        var tag = version ?? configuration["GitHubLinux:DefaultTag"] ?? "latest";
        return GetDownloadUrl(configuration, tag, "linux");
    }
}
