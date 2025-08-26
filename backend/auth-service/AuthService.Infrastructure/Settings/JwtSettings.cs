
using AuthService.Interfaces.Settings;

namespace AuthService.Infrastructure.Settings
{
    public class JwtSettings : IJwtSettings
    {
        public string Issuer { get; set; } = string.Empty;

        public string Audience { get; set; } = string.Empty;

        public string SecretKey { get; set; } = string.Empty;

        public int ExpirationInMinutes { get; set; }

        public int RefreshTokenExpirationInDays { get; set; }
    }
}
