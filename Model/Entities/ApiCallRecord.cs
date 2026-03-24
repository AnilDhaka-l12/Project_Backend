using Google.Cloud.Firestore;

namespace projectBackend.Model.Entities;

[FirestoreData]
public class ApiCallRecord
{
    [FirestoreProperty]
    public DateTime Timestamp { get; set; }
    
    [FirestoreProperty]
    public string Endpoint { get; set; } = string.Empty;
    
    [FirestoreProperty]
    public string Method { get; set; } = string.Empty;
    
    [FirestoreProperty]
    public string Payload { get; set; } = string.Empty;
    
    [FirestoreProperty]
    public string ResponseStatus { get; set; } = string.Empty;
}