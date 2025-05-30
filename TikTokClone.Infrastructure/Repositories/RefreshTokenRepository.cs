using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
using TikTokClone.Application.Interfaces.Repositories;
using TikTokClone.Domain.Entities;
using TikTokClone.Infrastructure.Data;

namespace TikTokClone.Infrastructure.Repositories
{
    public class RefreshTokenRepository : BaseRepository<RefreshToken>, IRefreshTokenRepository
    {
        public RefreshTokenRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<RefreshToken?> GetByTokenAsync(string token)
        {
            return await _dbSet.Include(rt => rt.User)
                .FirstOrDefaultAsync(rt => rt.Token == token);
        }

        public async Task<IEnumerable<RefreshToken>> GetByUserIdAsync(string userId)
        {
            return await _dbSet.Include(rt => rt.User)
                .Where(rt => rt.UserId == userId)
                .OrderByDescending(rt => rt.CreatedAt).ToListAsync();
        }

        public async Task<IEnumerable<RefreshToken>> GetExpiredTokensAsync()
        {
            return await _dbSet
                .Where(rt => rt.ExpiresAt < DateTime.UtcNow)
                .ToListAsync();
        }

        public async Task RemoveExpiredTokensAsync()
        {
            var expiredTokens = await GetExpiredTokensAsync();
            if (expiredTokens.Any())
            {
                _dbSet.RemoveRange(expiredTokens);
                await _context.SaveChangesAsync();
            }
        }

        public async Task RemoveUserTokensAsync(string userId)
        {
            var userTokens = await _dbSet.Where(rt => rt.UserId == userId).ToListAsync();
            if (userTokens.Any())
            {
                _dbSet.RemoveRange(userTokens);
                await _context.SaveChangesAsync();
            }
        }
    }
}
