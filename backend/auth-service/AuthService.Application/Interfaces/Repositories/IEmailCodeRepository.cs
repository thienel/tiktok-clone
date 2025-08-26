using AuthService.Domain.Entities;

namespace AuthService.Interfaces.Repositories
{
    public interface IEmailCodeRepository : IBaseRepository<EmailCode>
    {
        Task<EmailCode?> FindByEmailAsync(string email);
    }
}
