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

            if (FirebaseApp.DefaultInstance == null)
            {
                _firebaseApp = FirebaseApp.Create(new AppOptions()
                {
                    Credential = credential,
                    ProjectId = _firebaseSettings.ProjectId,
                });
            }
            else
            {
                _firebaseApp = FirebaseApp.DefaultInstance;
            }

            _firestoreDb = FirestoreDb.Create(_firebaseSettings.ProjectId);
            _storageClient = StorageClient.Create(credential);
        }

        #region  Firestore Operations
        public async Task<DocumentReference> AddDocumentAsync<T>(string collection, T data) where T : class
        {
            var collectionRef = _firestoreDb.Collection(collection);
            var documentRef = await collectionRef.AddAsync(data);

            return documentRef;
        }

        public async Task<T?> GetDocumentAsync<T>(string collection, string documentId) where T : class
        {
            var documentRef = _firestoreDb.Collection(collection).Document(documentId);
            var snapshot = await documentRef.GetSnapshotAsync();

            if (snapshot.Exists)
            {
                return snapshot.ConvertTo<T>();
            }

            return null;
        }

        public async Task UpdateDocumentAsync<T>(string collection, string documentId, T data) where T : class
        {
            var documentRef = _firestoreDb.Collection(collection).Document(documentId);
            await documentRef.SetAsync(data, SetOptions.MergeAll);
        }

        public async Task DeleteDocumentAsync(string collection, string documentId)
        {
            var documentRef = _firestoreDb.Collection(collection).Document(documentId);
            await documentRef.DeleteAsync();
        }

        public async Task<IEnumerable<T>> GetCollectionAsync<T>(string collection) where T : class
        {
            var snapshot = await _firestoreDb.Collection(collection).GetSnapshotAsync();
            var result = new List<T>();

            foreach (var document in snapshot.Documents)
            {
                if (document.Exists)
                {
                    result.Add(document.ConvertTo<T>());
                }
            }

            return result;
        }
        #endregion

        #region  Storage Operations
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

        #region  Helper Methods for TikTok Clone
        public Task<string> UploadVideoAsync(Stream videoStream, string originalFileName)
        {
            throw new NotImplementedException();
        }

        public Task<string> UploadImageAsync(Stream imageStream, string originalFileName)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region  Authentication Operations
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
