using TikTokClone.Domain.Entities;

namespace TikTokClone.Application.Interfaces.Services
{
    public interface ITokenService
    {
        string GenerateToken(User user);
        string GenerateRefreshToken();
        bool ValidateToken(string token);
    }
}
