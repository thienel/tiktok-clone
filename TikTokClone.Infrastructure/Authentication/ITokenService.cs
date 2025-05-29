using TikTokClone.Domain.Entities;

namespace TikTokClone.Infrastructure.Authentication
{
    public interface ITokenService
    {
        string GenerateToken(User user);
        string GenerateRefreshToken();
        bool ValidateToken(string token);
    }
}
