using Google.Cloud.Firestore;

namespace ProjectBackend.Model.Entities;

[FirestoreData]
public class User
{
    [FirestoreDocumentId]
    public string Id { get; set; } = string.Empty;

    [FirestoreProperty]
    public string Name { get; set; } = string.Empty;

    [FirestoreProperty]
    public string Surname { get; set; } = string.Empty;

    [FirestoreProperty]
    public string Email { get; set; } = string.Empty;

    [FirestoreProperty]
    public string Occupation { get; set; } = string.Empty;

    [FirestoreProperty]
    public string Organization { get; set; } = string.Empty;

    [FirestoreProperty]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [FirestoreProperty]
    public DateTime? UpdatedAt { get; set; }

    [FirestoreProperty]
    public bool IsActive { get; set; } = true;

    [FirestoreProperty]
    public string FullName => $"{Name} {Surname}";
}
