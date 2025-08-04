namespace TikTokClone.Application.Interfaces.Services
{
    public interface IEmailService
    {
        Task<bool> SendEmailVerificationCodeAsync(string email, string verificationCode);
        Task<bool> SendPasswordResetEmailAsync(string email, string verificationCode, string name);
    }
}
