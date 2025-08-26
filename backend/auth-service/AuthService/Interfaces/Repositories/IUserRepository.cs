using AuthService.Domain.Entities;

namespace AuthService.Interfaces.Repositories
{
    public interface IUserRepository : IBaseRepository<User>
    {
        Task<User?> GetByEmailAsync(string email);
        Task<User?> GetByUserNameAsync(string userName);
        Task<bool> IsEmailExistsAsync(string email);
        Task<bool> IsUserNameExistsAsync(string userName);
        Task<IEnumerable<User>> GetVerifiedUsersAsync();
        Task<User?> GetWithRefreshTokensAsync(string userId);
    }
}
