using TikTokClone.Domain.Entities;

namespace TikTokClone.Application.Interfaces.Repositories
{
    public interface IEmailVerificationRepository : IBaseRepository<EmailVerification>
    {
        Task<EmailVerification> FindByEmailAsync(string email);
    }
}
