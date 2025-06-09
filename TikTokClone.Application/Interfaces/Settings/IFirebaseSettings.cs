namespace TikTokClone.Application.Interfaces.Settings
{
    public interface IFirebaseSettings
    {
        string ProjectId { get; set; }
        string ServiceAccountKeyPath { get; set; }
        string StorageBucket { get; set; }
        string DatabaseUrl { get; set; }
    }
}
