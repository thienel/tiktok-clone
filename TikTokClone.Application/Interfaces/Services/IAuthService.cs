using TikTokClone.Application.DTOs;

namespace TikTokClone.Application.Interfaces.Services
{
    public interface IAuthService
    {
        Task<AuthResponseDto> LoginAsync(LoginRequestDto request);
        Task<AuthResponseDto> RegisterAsync(RegisterRequestDto request);
        Task<AuthResponseDto> RefreshTokenAsync(string refreshToken);
        Task<bool> LogoutAsync(string userId);
        Task<AuthResponseDto> ConfirmEmailAsync(string userId, string token);
        Task<bool> SendEmailConfirmationAsync(string email);
    }
}
