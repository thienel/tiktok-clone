
using TikTokClone.Application.DTOs;
using TikTokClone.Application.Interfaces.Repositories;
using TikTokClone.Application.Interfaces.Services;
using TikTokClone.Domain.Entities;

namespace TikTokClone.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public Task ChangeAvatarAsync(string userId, string avatarURL)
        {
            throw new NotImplementedException();
        }

        public Task<ProfileResponseDto> GetProfileAsync(string userId)
        {
            throw new NotImplementedException();
        }

        public Task Search(string value)
        {
            throw new NotImplementedException();
        }

        public Task<ProfileResponseDto> UpdateProfileAsync(User user)
        {
            throw new NotImplementedException();
        }
    }
}
