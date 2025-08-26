using AuthService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using AuthService.Interfaces.Repositories;
using AuthService.Domain.Entities;

namespace AuthService.Infrastructure.Repositories
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
