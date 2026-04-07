using Amazon.S3;
using Microsoft.Extensions.DependencyInjection;
using ProjectBackend.Config.Backblaze;

namespace ProjectBackend.Extensions;

public static class BackblazeRegistration
{
    /// <summary>
    /// Registers Backblaze B2 S3 services
    /// </summary>
    public static IServiceCollection RegisterBackblaze(this IServiceCollection services, IConfiguration configuration)
    {
        // Initialize Backblaze S3 Client
        var s3Client = BackblazeInitializer.InitializeS3Client(configuration);
        var bucketName = BackblazeInitializer.GetBucketName(configuration);

        // Register S3 client as a singleton (single instance for the entire app)
        services.AddSingleton(s3Client);
        services.AddSingleton(bucketName);

        return services;
    }
}