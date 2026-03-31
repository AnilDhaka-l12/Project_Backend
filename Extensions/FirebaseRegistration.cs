using Google.Cloud.Firestore;
using Microsoft.Extensions.DependencyInjection;
using projectBackend.Config.Firebase;

namespace projectBackend.Extensions;

public static class FirebaseRegistration
{
    /// <summary>
    /// Registers Firebase Firestore services
    /// </summary>
    public static IServiceCollection RegisterFirebase(this IServiceCollection services, IConfiguration configuration)
    {
        // Initialize Firebase Firestore
        var firestoreDb = FirebaseConfig.InitializeFirestore(configuration);

        // Register FirestoreDb as a singleton (single instance for the entire app)
        services.AddSingleton(firestoreDb);

        return services;
    }
}
