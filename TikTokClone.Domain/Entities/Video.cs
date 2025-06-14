namespace TikTokClone.Domain.Entities
{
    public class Video
    {
        public int Id { get; private set; }
        public string Title { get; private set; }
        public string Description { get; private set; }
        public bool IsVisible { get; private set; }
        public string Url { get; private set; }
        public string ThumbnailUrl { get; private set; }
        public string UserId { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime UpdatedAt { get; private set; }
        virtual public User User { get; private set; } = null!;

        public Video(string title, string description, string url, string thumbnailUrl, string userId)
        {
            Title = title;
            Description = description;
            Url = url;
            ThumbnailUrl = thumbnailUrl;
            UserId = userId;
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
            IsVisible = true;
        }

        public void ChangeVisibility(bool isVisible)
        {
            if (IsVisible == isVisible)
                return;
            IsVisible = isVisible;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
