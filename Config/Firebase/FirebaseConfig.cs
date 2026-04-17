using Google.Cloud.Firestore;
using Google.Apis.Auth.OAuth2;

namespace ProjectBackend.Config.Firebase;

public static class FirebaseConfig
{
    public static FirestoreDb InitializeFirestore(IConfiguration configuration)
    {
        var projectId = configuration["Firebase:ProjectId"]
            ?? throw new InvalidOperationException("Firebase ProjectId not found");

        // ✅ ONLY SOURCE: configuration (appsettings + env override)
        var credentialsPath = configuration["Firebase:CredentialsPath"];

        if (string.IsNullOrWhiteSpace(credentialsPath))
        {
            throw new InvalidOperationException("Firebase credentials path not found in configuration");
        }

        // Allow both relative + absolute paths
        var fullPath = Path.IsPathRooted(credentialsPath)
            ? credentialsPath
            : Path.Combine(AppContext.BaseDirectory, credentialsPath);

        if (!File.Exists(fullPath))
        {
            throw new FileNotFoundException($"Firebase credentials file not found: {fullPath}");
        }

        Console.WriteLine("✅ Using Firebase credentials file");
        Console.WriteLine($"📁 Path: {fullPath}");

        var credential = GoogleCredential.FromFile(fullPath);

        return new FirestoreDbBuilder
        {
            ProjectId = projectId,
            Credential = credential
        }.Build();
    }
}
