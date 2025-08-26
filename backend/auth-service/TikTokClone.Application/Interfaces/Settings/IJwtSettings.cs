namespace TikTokClone.Application.Interfaces.Settings
{
    public interface IJwtSettings
    {
        string Issuer { get; }
        string Audience { get; }
        string SecretKey { get; }
        int ExpirationInMinutes { get; }
        int RefreshTokenExpirationInDays { get; }
    }
}
