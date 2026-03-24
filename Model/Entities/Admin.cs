using Google.Cloud.Firestore;

namespace projectBackend.Model.Entities;

[FirestoreData]
public class Admin
{
    [FirestoreDocumentId]
    public string Id { get; set; } = string.Empty;
    
    [FirestoreProperty]
    public string Username { get; set; } = string.Empty;
    
    [FirestoreProperty]
    public string Password { get; set; } = string.Empty;
    
    [FirestoreProperty]
    public string Role { get; set; } = "Admin";
    
    [FirestoreProperty]
    public List<ApiCallRecord> ApiCallHistory { get; set; } = new();
}