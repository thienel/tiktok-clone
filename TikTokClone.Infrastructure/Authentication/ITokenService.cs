using TikTokClone.Domain.Entities;

namespace TikTokClone.Infrastructure.Authentication
{
    public interface ITokenService
    {
        Task<string> GenerateTokenAsync(User user);
        Task<string> GenerateRefreshTokenAsync();
        bool ValidateToken(string token);
    }
}
