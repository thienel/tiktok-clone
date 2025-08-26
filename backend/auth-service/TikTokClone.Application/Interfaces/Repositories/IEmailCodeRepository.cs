using TikTokClone.Domain.Entities;

namespace TikTokClone.Application.Interfaces.Repositories
{
    public interface IEmailCodeRepository : IBaseRepository<EmailCode>
    {
        Task<EmailCode?> FindByEmailAsync(string email);
    }
}
