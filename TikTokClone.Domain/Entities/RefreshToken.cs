namespace TikTokClone.Domain.Entities
{
    public class RefreshToken
    {
        public int Id { get; set; }
        public string Token { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public DateTime ExpiresAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? RevokedAt { get; set; }
        public string? ReplacedByToken { get; set; }
        public string? RevokedByIp { get; set; }
        public string? CreatedByIp { get; set; }

        // Navigation property
        public virtual User User { get; set; } = null!;

        // Computed properties
        public bool IsExpired => DateTime.UtcNow >= ExpiresAt;
        public bool IsRevoked => RevokedAt.HasValue;
        public bool IsActive => !IsRevoked && !IsExpired;

        // Domain methods
        public void Revoke(string? replacedByToken = null, string? revokedByIp = null)
        {
            RevokedAt = DateTime.UtcNow;
            ReplacedByToken = replacedByToken;
            RevokedByIp = revokedByIp;
        }

        public bool CanBeRefreshed()
        {
            return IsActive;
        }
    }
}
