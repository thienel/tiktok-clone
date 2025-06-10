
using TikTokClone.Domain.Exceptions;

namespace TikTokClone.Application.DTOs
{
    public class LoginRequestDto
    {
        public string UsernameOrEmail { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public class RegisterRequestDto
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public DateOnly BirthDate { get; set; }
        public string VerificationCode { get; set; } = string.Empty;
    }

    public class ResetPasswordRequestDto
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string VerificationCode { get; set; } = string.Empty;
    }

    public class AuthResponseDto
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; } = string.Empty;
        public string? ErrorCode { get; set; }
        public string? Token { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime? ExpiresAt { get; set; }
        public ProfileResponseDto? User { get; set; }
    }

    public class RefreshTokenRequestDto
    {
        public string RefreshToken { get; set; } = string.Empty;
    }

    public class SendVerificationCodeDto
    {
        public string Email { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
    }

    public class CheckUsernameDto
    {
        public string Username { get; set; } = string.Empty;
    }

    public class ChangeUsernameDto
    {
        public string Email { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
    }

    public class CheckBirthdateDto
    {
        public DateOnly BirthDate { get; set; }
    }
}
