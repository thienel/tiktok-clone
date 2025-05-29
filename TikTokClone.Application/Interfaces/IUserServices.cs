
using Microsoft.AspNetCore.Identity;
using TikTokClone.Domain.Entities;

namespace TikTokClone.Application.Interfaces
{
    public interface IUserService
    {
        Task<IdentityResult> CreateUserAsync(string email, string firstName, string lastName, string password);
        Task<User?> GetUserByIdAsync(string userId);
        Task<IdentityResult> UpdateUserProfileAsync(string userId, string firstName, string lastName, string? bio);
    }
}
