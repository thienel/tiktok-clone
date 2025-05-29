
using Microsoft.AspNetCore.Identity;
using TikTokClone.Application.Interfaces;
using TikTokClone.Domain.Entities;

namespace TikTokClone.Application.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<User> _userManager;

        public UserService(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        public async Task<IdentityResult> CreateUserAsync(string email, string firstName, string lastName, string password)
        {
            var user = new User(email: email, firstName: firstName, lastName: lastName);
            return await _userManager.CreateAsync(user, password);
        }

        public async Task<User?> GetUserByIdAsync(string userId)
        {
            return await _userManager.FindByIdAsync(userId);
        }

        public async Task<IdentityResult> UpdateUserProfileAsync(string userId, string firstName, string lastName, string? bio)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return IdentityResult.Failed(new IdentityError { Description = "User not found" });

            user.ChangeName(firstName, lastName);
            user.ChangeBio(bio);

            return await _userManager.UpdateAsync(user);
        }
    }
}
