namespace AuthService.Domain.Entities
{
    public class RefreshToken
    {
        public int Id { get; private set; }
        public string Token { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public DateTime ExpiresAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? RevokedAt { get; set; }
        public string? ReplacedByToken { get; set; }

        public virtual User User { get; set; } = null!;

        public bool IsExpired => DateTime.UtcNow >= ExpiresAt;
        public bool IsRevoked => RevokedAt.HasValue;
        public bool IsActive => !IsRevoked && !IsExpired;

        public void Revoke(string? replacedByToken = null)
        {
            RevokedAt = DateTime.UtcNow;
            ReplacedByToken = replacedByToken;
        }

        public bool CanBeRefreshed()
        {
            return IsActive;
        }
    }
}
