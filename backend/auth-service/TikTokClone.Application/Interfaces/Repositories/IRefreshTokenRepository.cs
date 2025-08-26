using TikTokClone.Domain.Entities;

namespace TikTokClone.Application.Interfaces.Repositories
{
    public interface IRefreshTokenRepository : IBaseRepository<RefreshToken>
    {
        Task<RefreshToken?> GetByTokenAsync(string token);
        Task<IEnumerable<RefreshToken>> GetByUserIdAsync(string userId);
        Task<IEnumerable<RefreshToken>> GetExpiredTokensAsync();
        Task RemoveExpiredTokensAsync();
        Task RemoveUserTokensAsync(string userId);
        Task<IEnumerable<RefreshToken>> GetActiveByUserIdAsync(string userId);
    }
}
