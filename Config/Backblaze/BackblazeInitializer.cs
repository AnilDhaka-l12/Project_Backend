using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.Extensions.Configuration;
using System.Net;

namespace ProjectBackend.Config.Backblaze
{
    public static class BackblazeInitializer
    {
        public static IAmazonS3 InitializeS3Client(IConfiguration configuration)
        {
            var accessKey = configuration["BackblazeSettings:AccessKey"] 
                ?? throw new InvalidOperationException("Backblaze:AccessKey is required");
            
            var secretKey = configuration["BackblazeSettings:SecretKey"] 
                ?? throw new InvalidOperationException("Backblaze:SecretKey is required");
            
            var endpointUrl = configuration["BackblazeSettings:EndpointUrl"] 
                ?? throw new InvalidOperationException("Backblaze:EndpointUrl is required");

            var bucketName = configuration["BackblazeSettings:BucketName"] 
                ?? throw new InvalidOperationException("Backblaze:BucketName is required");

            Console.WriteLine($"=== BACKBLAZE DEBUG INFO ===");
            Console.WriteLine($"🔑 AccessKey (first 10 chars): {accessKey.Substring(0, Math.Min(10, accessKey.Length))}...");
            Console.WriteLine($"🔐 SecretKey (first 10 chars): {secretKey.Substring(0, Math.Min(10, secretKey.Length))}...");
            Console.WriteLine($"🌐 EndpointUrl: {endpointUrl}");
            Console.WriteLine($"📦 BucketName: {bucketName}");
            Console.WriteLine($"==============================");

            var config = new AmazonS3Config
            {
                ServiceURL = endpointUrl,
                ForcePathStyle = true,
                UseHttp = false
            };

            var client = new AmazonS3Client(accessKey, secretKey, config);
            
            // TEST 1: List buckets (tests basic auth)
            Console.WriteLine("\n📋 TEST 1: Listing buckets...");
            try
            {
                var listBucketsResponse = client.ListBucketsAsync().GetAwaiter().GetResult();
                Console.WriteLine($"✅ SUCCESS! Found {listBucketsResponse.Buckets.Count} buckets:");
                foreach (var bucket in listBucketsResponse.Buckets)
                {
                    Console.WriteLine($"   - {bucket.BucketName}");
                }
            }
            catch (AmazonS3Exception ex)
            {
                Console.WriteLine($"❌ ListBuckets FAILED: {ex.Message}");
                Console.WriteLine($"   Error Code: {ex.ErrorCode}");
                Console.WriteLine($"   Status Code: {ex.StatusCode}");
            }
            
            // TEST 2: List objects in your specific bucket
            Console.WriteLine($"\n📋 TEST 2: Listing objects in bucket '{bucketName}'...");
            try
            {
                var listRequest = new ListObjectsV2Request 
                { 
                    BucketName = bucketName,
                    MaxKeys = 10
                };
                var response = client.ListObjectsV2Async(listRequest).GetAwaiter().GetResult();
                Console.WriteLine($"✅ SUCCESS! Found {response.KeyCount} files:");
                foreach (var obj in response.S3Objects)
                {
                    Console.WriteLine($"   - {obj.Key} ({obj.Size} bytes)");
                }
            }
            catch (AmazonS3Exception ex)
            {
                Console.WriteLine($"❌ ListObjects FAILED: {ex.Message}");
                Console.WriteLine($"   Error Code: {ex.ErrorCode}");
                Console.WriteLine($"   Status Code: {ex.StatusCode}");
                Console.WriteLine($"   Request ID: {ex.RequestId}");
            }
            
            // TEST 3: Try to get metadata of your specific file
            Console.WriteLine($"\n📋 TEST 3: Getting file metadata for 'video.mov'...");
            try
            {
                var metadataRequest = new GetObjectMetadataRequest
                {
                    BucketName = bucketName,
                    Key = "video.mov"
                };
                var metadata = client.GetObjectMetadataAsync(metadataRequest).GetAwaiter().GetResult();
                Console.WriteLine($"✅ SUCCESS! File exists:");
                Console.WriteLine($"   Size: {metadata.ContentLength} bytes");
                Console.WriteLine($"   Type: {metadata.Headers.ContentType}");
                Console.WriteLine($"   Modified: {metadata.LastModified}");
            }
            catch (AmazonS3Exception ex)
            {
                Console.WriteLine($"❌ GetMetadata FAILED: {ex.Message}");
                Console.WriteLine($"   Error Code: {ex.ErrorCode}");
                Console.WriteLine($"   Status Code: {ex.StatusCode}");
            }
            
            // TEST 4: Generate a pre-signed URL
            Console.WriteLine($"\n📋 TEST 4: Generating pre-signed URL for 'video.mov'...");
            try
            {
                var urlRequest = new GetPreSignedUrlRequest
                {
                    BucketName = bucketName,
                    Key = "video.mov",
                    Expires = DateTime.UtcNow.AddMinutes(5),
                    Verb = HttpVerb.GET
                };
                var url = client.GetPreSignedURL(urlRequest);
                Console.WriteLine($"✅ SUCCESS! Pre-signed URL generated:");
                Console.WriteLine($"   URL: {url}");
            }
            catch (AmazonS3Exception ex)
            {
                Console.WriteLine($"❌ GenerateUrl FAILED: {ex.Message}");
                Console.WriteLine($"   Error Code: {ex.ErrorCode}");
            }
            
            Console.WriteLine("\n=== DEBUG COMPLETE ===");
            
            return client;
        }
        
        public static string GetBucketName(IConfiguration configuration)
        {
            return configuration["BackblazeSettings:BucketName"] 
                ?? throw new InvalidOperationException("Backblaze:BucketName is required");
        }
    }
}