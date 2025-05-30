
using TikTokClone.Application.Interfaces.Repositories;
using TikTokClone.Application.Interfaces.Services;

namespace TikTokClone.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }
    }
}
