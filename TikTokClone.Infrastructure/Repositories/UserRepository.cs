using TikTokClone.Application.Interfaces.Repositories;
using TikTokClone.Domain.Entities;

namespace TikTokClone.Infrastructure.Repositories
{
    public class UserRepository : BaseRepository<User>, IUserRepository
    {
        public Task<User?> GetByEmailAsync(string email)
        {
            throw new NotImplementedException();
        }
        public Task<User?> GetByUserNameAsync(string userName)
        {
            throw new NotImplementedException();
        }
        public Task<bool> IsEmailExistsAsync(string email)
        {
            throw new NotImplementedException();
        }
        public Task<bool> IsUserNameExistsAsync(string userName)
        {
            throw new NotImplementedException();
        }
        public Task<IEnumerable<User>> GetVerifiedUsersAsync()
        {
            throw new NotImplementedException();
        }
        public Task<User?> GetWithRefreshTokensAsync(string userId)
        {
            throw new NotImplementedException();
        }
    }
}
