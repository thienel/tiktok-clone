using Google.Cloud.Firestore;

namespace TikTokClone.Application.Interfaces.Services
{
    public interface IFirebaseService
    {
        // Firestore Operations
        Task<DocumentReference> AddDocumentAsync<T>(string collection, T data) where T : class;
        Task<T?> GetDocumentAsync<T>(string collection, string documentId) where T : class;
        Task UpdateDocumentAsync<T>(string collection, string documentId, T data) where T : class;
        Task DeleteDocumentAsync(string collection, string documentId);
        Task<IEnumerable<T>> GetCollectionAsync<T>(string collection) where T : class;

        // Storage Operations
        Task<string> UploadFileAsync(Stream fileStream, string fileName, string contentType);
        Task<Stream> DownloadFileAsync(string fileName);
        Task DeleteFileAsync(string fileName);

        // Helper Methods for TikTok Clone
        Task<string> UploadImageAsync(Stream imageStream, string originalFileName);

        // Authentication Operations
        Task<string> CreateCustomTokenAsync(string uid, Dictionary<string, object>? additionalClaims = null);
        Task<FirebaseAdmin.Auth.UserRecord> GetUserAsync(string uid);
        Task<FirebaseAdmin.Auth.UserRecord> CreateUserAsync(FirebaseAdmin.Auth.UserRecordArgs args);
    }
}
