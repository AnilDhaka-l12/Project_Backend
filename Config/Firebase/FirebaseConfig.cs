using Google.Cloud.Firestore;
using Google.Apis.Auth.OAuth2;

namespace ProjectBackend.Config.Firebase;

public static class FirebaseConfig
{
    public static FirestoreDb InitializeFirestore(IConfiguration configuration)
    {
        var projectId = configuration["Firebase:ProjectId"]
            ?? throw new InvalidOperationException("Firebase ProjectId not found");

        // 1. Try environment variable first (Cloud Run)
        var firebaseJson = Environment.GetEnvironmentVariable("FIREBASE_SERVICE_ACCOUNT_JSON");

        if (!string.IsNullOrEmpty(firebaseJson))
        {
            Console.WriteLine("✅ Using Firebase credentials from ENV variable");

            var credential = GoogleCredential.FromJson(firebaseJson);

            return new FirestoreDbBuilder
            {
                ProjectId = projectId,
                Credential = credential
            }.Build();
        }

        // 2. Fallback to file (local Docker / dev)
        var credentialsPath = configuration["Firebase:CredentialsPath"];

        if (!string.IsNullOrEmpty(credentialsPath))
        {
            var fullPath = Path.Combine(Directory.GetCurrentDirectory(), credentialsPath);

            if (File.Exists(fullPath))
            {
                Console.WriteLine("✅ Using Firebase credentials from file");

                Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", fullPath);

                return FirestoreDb.Create(projectId);
            }
        }

        // 3. Optional: fallback to default credentials (Cloud Run IAM)
        Console.WriteLine("⚠️ Falling back to default Google credentials");

        return FirestoreDb.Create(projectId);
    }
}