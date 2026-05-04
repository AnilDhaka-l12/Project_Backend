using Microsoft.Extensions.Configuration;
using System;

namespace ProjectBackend.Helper;

public static class GitHubUrlHelper
{
    public static string GetDownloadUrl(IConfiguration configuration, string version, string platform)
    {
        // Validate that version is not null or empty
        if (string.IsNullOrWhiteSpace(version))
        {
            throw new ArgumentException("Version/Tag cannot be null or empty", nameof(version));
        }

        // Determine which config section to use based on platform
        var configSection = platform?.ToLower() == "windows" ? "GitHub" : "GitHubLinux";

        var owner = configuration[$"{configSection}:Owner"]
            ?? throw new InvalidOperationException($"{configSection}:Owner not configured");

        var repo = configuration[$"{configSection}:Repo"]
            ?? throw new InvalidOperationException($"{configSection}:Repo not configured");

        var fileName = configuration[$"{configSection}:fileName"]
            ?? throw new InvalidOperationException($"{configSection}:fileName not configured");

        // Use the passed version parameter (not the config's DefaultTag)
        var tag = version;

        // For "latest" tag
        if (tag.Equals("latest", StringComparison.OrdinalIgnoreCase))
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