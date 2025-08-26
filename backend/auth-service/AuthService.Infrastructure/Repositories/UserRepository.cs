using AuthService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using AuthService.Interfaces.Repositories;
using AuthService.Domain.Entities;

namespace AuthService.Infrastructure.Repositories
{
    public class UserRepository : BaseRepository<User>, IUserRepository
    {
        public UserRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _dbSet.FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<User?> GetByUserNameAsync(string userName)
        {
            return await _dbSet.FirstOrDefaultAsync(u => u.UserName == userName);
        }

        public async Task<bool> IsEmailExistsAsync(string email)
        {
            return await _dbSet.AnyAsync(u => u.Email == email);
        }

        public async Task<bool> IsUserNameExistsAsync(string userName)
        {
            return await _dbSet.AnyAsync(u => u.UserName == userName);
        }

        public async Task<IEnumerable<User>> GetVerifiedUsersAsync()
        {
            return await _dbSet.Where(u => u.IsVerified).ToListAsync();
        }

        public async Task<User?> GetWithRefreshTokensAsync(string userId)
        {
            return await _dbSet.Include(u => u.RefreshTokens)
                .FirstOrDefaultAsync(u => u.Id == userId);
        }
    }
}
