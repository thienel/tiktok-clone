using Microsoft.EntityFrameworkCore;
using TikTokClone.Application.Interfaces.Repositories;
using TikTokClone.Domain.Entities;
using TikTokClone.Infrastructure.Data;

namespace TikTokClone.Infrastructure.Repositories
{
    public class EmailCodeRepository : BaseRepository<EmailCode>, IEmailCodeRepository
    {
        public EmailCodeRepository(AppDbContext context) : base(context)
        {

        }
        public async Task<EmailCode?> FindByEmailAsync(string email)
        {
            return await _dbSet.FirstOrDefaultAsync(e => e.Email == email);
        }
    }
}
