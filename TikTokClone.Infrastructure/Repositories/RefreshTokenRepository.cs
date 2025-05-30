using TikTokClone.Application.Interfaces.Repositories;
using TikTokClone.Domain.Entities;

namespace TikTokClone.Infrastructure.Repositories
{
    public class RefreshTokenRepository : BaseRepository<RefreshToken>, IRefreshTokenRepository
    {
        public Task<RefreshToken?> GetByTokenAsync(string token)
        {
            throw new NotImplementedException();
        }
        public Task<IEnumerable<RefreshToken>> GetByUserIdAsync(string userId)
        {
            throw new NotImplementedException();
        }
        public Task<IEnumerable<RefreshToken>> GetExpiredTokensAsync()
        {
            throw new NotImplementedException();
        }
        public Task RemoveExpiredTokensAsync()
        {
            throw new NotImplementedException();
        }
        public Task RemoveUserTokensAsync(string userId)
        {
            throw new NotImplementedException();
        }
    }
}
