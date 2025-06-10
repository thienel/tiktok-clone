using TikTokClone.Application.Interfaces.Settings;

namespace TikTokClone.Infrastructure.Settings
{
    public class FirebaseSettings : IFirebaseSettings
    {
        public string ProjectId { get; set; } = string.Empty;
        public string ServiceAccountKeyPath { get; set; } = string.Empty;
        public string StorageBucket { get; set; } = string.Empty;
        public string DatabaseUrl { get; set; } = string.Empty;
    }
}
