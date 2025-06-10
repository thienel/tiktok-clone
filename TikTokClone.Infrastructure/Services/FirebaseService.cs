using FirebaseAdmin;
using FirebaseAdmin.Auth;
using Google.Api;
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
        public async Task<string> UploadFileAsync(Stream fileStream, string fileName, string contentType)
        {
            try
            {
                var uniqueFileName = $"{Guid.NewGuid()}_{fileName}";

                var storageObject = await _storageClient.UploadObjectAsync(
                    bucket: _firebaseSettings.StorageBucket,
                    objectName: uniqueFileName,
                    contentType: contentType,
                    source: fileStream
                );

                var downloadUrl = $"https://firebasestorage.googleapis.com/v0/b/{_firebaseSettings.StorageBucket}/o/{Uri.EscapeDataString(uniqueFileName)}?alt=media";

                return downloadUrl;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error uploading file: {ex.Message}", ex);
            }
        }

        public async Task<Stream> DownloadFileAsync(string fileName)
        {
            try
            {
                var stream = new MemoryStream();
                await _storageClient.DownloadObjectAsync(
                    bucket: _firebaseSettings.StorageBucket,
                    objectName: fileName,
                    destination: stream
                );

                stream.Position = 0;
                return stream;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error downloading file: {ex.Message}", ex);
            }
        }

        public async Task DeleteFileAsync(string fileName)
        {
            try
            {
                await _storageClient.DeleteObjectAsync(
                    bucket: _firebaseSettings.StorageBucket,
                    objectName: fileName
                );
            }
            catch (Exception ex)
            {
                throw new Exception($"Error deleting file: {ex.Message}", ex);
            }
        }
        #endregion

        #region Authentication Operations
        public async Task<string> CreateCustomTokenAsync(string uid, Dictionary<string, object>? additionalClaims = null)
        {
            try
            {
                var customToken = await FirebaseAuth.DefaultInstance.CreateCustomTokenAsync(uid, additionalClaims);
                return customToken;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error creating custom token: {ex.Message}", ex);
            }
        }

        public async Task<UserRecord> GetUserAsync(string uid)
        {
            try
            {
                var userRecord = await FirebaseAuth.DefaultInstance.GetUserAsync(uid);
                return userRecord;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error getting user: {ex.Message}", ex);
            }
        }

        public async Task<UserRecord> CreateUserAsync(UserRecordArgs args)
        {
            try
            {
                var userRecord = await FirebaseAuth.DefaultInstance.CreateUserAsync(args);
                return userRecord;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error creating user: {ex.Message}", ex);
            }
        }
        #endregion

        #region  Helper Methods for TikTok Clone
        public async Task<string> UploadVideoAsync(Stream videoStream, string originalFileName)
        {
            var contentType = GetContentType(originalFileName);
            if (!IsVideoFile(contentType))
            {
                throw new ArgumentException("File must be a video");
            }

            return await UploadFileAsync(videoStream, originalFileName, contentType);
        }

        public async Task<string> UploadImageAsync(Stream imageStream, string originalFileName)
        {
            var contentType = GetContentType(originalFileName);
            if (!IsImageFile(contentType))
            {
                throw new ArgumentException("File must be an image");
            }

            return await UploadFileAsync(imageStream, originalFileName, contentType);
        }

        private string GetContentType(string fileName)
        {
            var extension = Path.GetExtension(fileName).ToLowerInvariant();
            return extension switch
            {
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".gif" => "image/gif",
                ".webp" => "image/webp",
                ".mp4" => "video/mp4",
                ".avi" => "video/avi",
                ".mov" => "video/quicktime",
                ".wmv" => "video/x-ms-wmv",
                ".webm" => "video/webm",
                _ => "application/octet-stream"
            };
        }

        private bool IsImageFile(string contentType)
        {
            return contentType.StartsWith("image/");
        }

        private bool IsVideoFile(string contentType)
        {
            return contentType.StartsWith("video/");
        }
        #endregion

    }
}
