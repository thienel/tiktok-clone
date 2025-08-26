using AuthService.Domain.Entities;

namespace AuthService.Interfaces.Services
{
    public interface ITokenService
    {
        string GenerateToken(User user);
        string GenerateRefreshToken();
        bool ValidateToken(string token);
    }
}
