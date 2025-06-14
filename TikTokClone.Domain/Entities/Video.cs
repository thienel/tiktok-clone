namespace TikTokClone.Domain.Entities
{
    public class Video
    {
        public int Id { get; private set; }
        public string? Title { get; private set; }
        public string? Description { get; private set; }
        public bool IsVisible { get; private set; }
        public string Url { get; private set; }
        public string ThumbnailUrl { get; private set; }
        public string UserId { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime UpdatedAt { get; private set; }
        virtual public User User { get; private set; } = null!;

        public const int MaxDescriptionLength = 4000;
        public const int MaxTitleLength = 100;

        public Video(string title, string description, string url, string thumbnailUrl, string userId)
        {
            if (!IsValidDescription(description))
            {
                throw new Exception();
            }
            if (!IsValidTitle(title))
            {
                throw new Exception();
            }
            if (!IsValidUrl(url))
            {
                throw new Exception();
            }
            if (!IsValidUrl(thumbnailUrl))
            {
                throw new Exception();
            }
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

        private bool IsValidTitle(string title)
        {
            return title.Trim().Length <= MaxTitleLength;
        }

        private bool IsValidDescription(string description)
        {
            return description.Trim().Length <= MaxDescriptionLength;
        }

        private bool IsValidUrl(string url)
        {
            return Uri.TryCreate(url, UriKind.Absolute, out _);
        }
    }
}
