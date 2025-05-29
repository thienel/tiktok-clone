using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using TikTokClone.Domain.Entities;

namespace TikTokClone.Infrastructure.Authentication
{
    public class TokenService : ITokenService
    {
        private readonly JwtSettings _jwtSettings;
        private readonly UserManager<User> _userManager;

        public TokenService(IOptions<JwtSettings> jwtSettings, UserManager<User> userManager)
        {
            _jwtSettings = jwtSettings.Value;
            _userManager = userManager;
        }
        public Task<string> GenerateRefreshTokenAsync()
        {
            throw new NotImplementedException();
        }

        public Task<string> GenerateTokenAsync(User user)
        {
            throw new NotImplementedException();
        }

        public bool ValidateToken(string token)
        {
            throw new NotImplementedException();
        }
    }

}
