using AuthService.DTOs;

namespace AuthService.Interfaces.Services
{
    public interface IAuthService
    {
        Task<AuthResponseDto> LoginAsync(LoginRequestDto request);
        Task<AuthResponseDto> RegisterAsync(RegisterRequestDto request);
        Task<AuthResponseDto> ResetPasswordAsync(ResetPasswordRequestDto request);
        Task<AuthResponseDto> RefreshTokenAsync(string refreshToken);
        Task<AuthResponseDto> LogoutAsync(string userId);
        Task<AuthResponseDto> SendEmailCodeAsync(string email, string type);
    }
}
