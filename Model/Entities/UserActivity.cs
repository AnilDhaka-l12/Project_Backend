using Google.Cloud.Firestore;

namespace projectBackend.Model.Entities;

[FirestoreData]
public class UserActivity
{
    [FirestoreDocumentId]
    public string Id { get; set; } = string.Empty;

    [FirestoreProperty]
    public string UserId { get; set; } = string.Empty;

    [FirestoreProperty]
    public DateTime LastLoggedIn { get; set; }

    [FirestoreProperty]
    public double LoggedHour { get; set; }

    [FirestoreProperty]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    [FirestoreProperty]
    public DateTime Date { get; set; }

    [FirestoreProperty]
    public int LoginCount { get; set; } = 0;
}