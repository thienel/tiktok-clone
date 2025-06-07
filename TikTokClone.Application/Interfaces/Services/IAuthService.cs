using TikTokClone.Application.DTOs;

namespace TikTokClone.Application.Interfaces.Services
{
    public interface IAuthService
    {
        Task<AuthResponseDto> LoginAsync(LoginRequestDto request);
        Task<AuthResponseDto> RegisterAsync(RegisterRequestDto request);
        Task<AuthResponseDto> RefreshTokenAsync(string refreshToken);
        Task<AuthResponseDto> LogoutAsync(string userId);
        Task<AuthResponseDto> SendEmailVerificationCodeAsync(string email);
        Task<AuthResponseDto> CheckValidUsername(string username);
    }
}
