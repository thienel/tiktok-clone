using TikTokClone.Application.Interfaces.Services;

namespace TikTokClone.Application.Services
{
    public class EmailService : IEmailService
    {
        public Task<bool> SendEmailVerificationCodeAsync(string email, string mail, string SendEmailVerificationCodeAsync)
        {
            throw new NotImplementedException();
        }

        public Task<bool> SendWelcomeEmailAsync(string email, string nameSendEmailVerificationCodeAsync)
        {
            throw new NotImplementedException();
        }

        public Task<bool> SendPasswordResetEmailAsync(string email, string resetTokenSendEmailVerificationCodeAsync)
        {
            throw new NotImplementedException();
        }
    }
}
