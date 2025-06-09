using System.ComponentModel.DataAnnotations;
using TikTokClone.Application.DTOs;

namespace TikTokClone.Application.Interfaces.Services
{
    public interface IAuthService
    {
        Task<AuthResponseDto> LoginAsync(LoginRequestDto request);
        Task<AuthResponseDto> RegisterAsync(RegisterRequestDto request);
        Task<AuthResponseDto> ResetPasswordAsync(ResetPasswordRequestDto request);
        Task<AuthResponseDto> RefreshTokenAsync(string refreshToken);
        Task<AuthResponseDto> LogoutAsync(string userId);
        Task<AuthResponseDto> SendEmailCodeAsync(string email, string type);
        Task<AuthResponseDto> CheckValidUsernameAsync(string username);
        Task<AuthResponseDto> ChangeUsernameAsync(string email, string username);
        AuthResponseDto CheckValidBirthDate(DateOnly birthDate);
    }
}
