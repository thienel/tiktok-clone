using Microsoft.AspNetCore.Identity;
using TikTokClone.Application.DTOs;
using TikTokClone.Application.Interfaces;
using TikTokClone.Domain.Entities;

namespace TikTokClone.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<User> _userManager;

        public AuthService()
        {

        }
        public Task<AuthResponseDto> LoginAsync(LoginRequestDto request)
        {
            throw new NotImplementedException();
        }

        public Task<bool> LogoutAsync(string userId)
        {
            throw new NotImplementedException();
        }

        public Task<AuthResponseDto> RefreshTokenAsync(string refreshToken)
        {
            throw new NotImplementedException();
        }

        public Task<AuthResponseDto> RegisterAsync(RegisterRequestDto request)
        {
            throw new NotImplementedException();
        }
    }
}
