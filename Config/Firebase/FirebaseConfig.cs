using Google.Cloud.Firestore;

namespace projectBackend.Config.Firebase;

public static class FirebaseConfig
{
    public static FirestoreDb InitializeFirestore(IConfiguration configuration)
    {
        var projectId = configuration["Firebase:ProjectId"]
            ?? throw new InvalidOperationException("Firebase ProjectId not found in configuration");

        var credentialsPath = configuration["Firebase:CredentialsPath"]
            ?? throw new InvalidOperationException("Firebase credentials path not found in configuration");

        // Get the absolute path
        var fullPath = Path.Combine(Directory.GetCurrentDirectory(), credentialsPath);

        if (!File.Exists(fullPath))
        {
            throw new FileNotFoundException($"Firebase credentials file not found at: {fullPath}");
        }

        // Set environment variable for Google Cloud authentication
        Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", fullPath);

        // Initialize Firestore
        var firestoreDb = FirestoreDb.Create(projectId);

        Console.WriteLine($"✅ Firebase Firestore initialized successfully!");
        Console.WriteLine($"   Project ID: {projectId}");
        Console.WriteLine($"   Credentials: {fullPath}");

        return firestoreDb;
    }
}
