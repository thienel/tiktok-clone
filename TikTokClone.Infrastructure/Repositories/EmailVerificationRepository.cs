using Microsoft.EntityFrameworkCore;
using TikTokClone.Application.Interfaces.Repositories;
using TikTokClone.Domain.Entities;
using TikTokClone.Infrastructure.Data;

namespace TikTokClone.Infrastructure.Repositories
{
    public class EmailVerificationRepository : BaseRepository<EmailVerification>, IEmailVerificationRepository
    {
        public EmailVerificationRepository(AppDbContext context) : base(context)
        {

        }
        public async Task<EmailVerification?> FindByEmailAsync(string email)
        {
            return await _dbSet.FirstOrDefaultAsync(e => e.Email == email);
        }
    }
}
