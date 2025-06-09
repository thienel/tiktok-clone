using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Firestore;
using Google.Cloud.Storage.V1;
using TikTokClone.Application.Interfaces.Services;
using TikTokClone.Application.Interfaces.Settings;

namespace TikTokClone.Infrastructure.Services
{
    public class FirebaseService : IFirebaseService
    {
        private readonly FirestoreDb _firestoreDb;
        private readonly StorageClient _storageClient;
        private readonly IFirebaseSettings _firebaseSettings;
        private readonly FirebaseApp _firebaseApp;

        public FirebaseService(IFirebaseSettings firebaseSettings)
        {
            _firebaseSettings = firebaseSettings;

            var credential = GoogleCredential.FromFile(_firebaseSettings.ServiceAccountKeyPath);
            _firebaseApp = FirebaseApp.Create(new AppOptions()
            {
                Credential = credential,
                ProjectId = _firebaseSettings.ProjectId,
            });

            _firestoreDb = FirestoreDb.Create(_firebaseSettings.ProjectId);
            _storageClient = StorageClient.Create(credential);
        }

        #region Firestore Operations
        public Task<DocumentReference> AddDocumentAsync<T>(string collection, T data) where T : class
        {
            throw new NotImplementedException();
        }
        public Task<T?> GetDocumentAsync<T>(string collection, string documentId) where T : class
        {
            throw new NotImplementedException();
        }
        public Task UpdateDocumentAsync<T>(string collection, string documentId, T data) where T : class
        {
            throw new NotImplementedException();
        }
        public Task DeleteDocumentAsync(string collection, string documentId)
        {
            throw new NotImplementedException();
        }
        public Task<IEnumerable<T>> GetCollectionAsync<T>(string collection) where T : class
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Storage Operations
        public Task<string> UploadFileAsync(Stream fileStream, string fileName, string contentType)
        {
            throw new NotImplementedException();
        }
        public Task<Stream> DownloadFileAsync(string fileName)
        {
            throw new NotImplementedException();
        }
        public Task DeleteFileAsync(string fileName)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Authentication Operations
        public Task<string> CreateCustomTokenAsync(string uid, Dictionary<string, object>? additionalClaims = null)
        {
            throw new NotImplementedException();
        }
        public Task<FirebaseAdmin.Auth.UserRecord> GetUserAsync(string uid)
        {
            throw new NotImplementedException();
        }
        public Task<FirebaseAdmin.Auth.UserRecord> CreateUserAsync(FirebaseAdmin.Auth.UserRecordArgs args)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
