namespace TikTokClone.Application.Interfaces.Services
{
    public interface IEmailService
    {
        Task<bool> SendEmailVerificationCodeAsync(string email, string mail, string verificationCode);
        Task<bool> SendWelcomeEmailAsync(string email, string name);
        Task<bool> SendPasswordResetEmailAsync(string email, string resetToken);
    }
}
