using Microsoft.AspNetCore.Identity;
using TikTokClone.Application.DTOs;
using TikTokClone.Application.Interfaces;
using TikTokClone.Domain.Entities;
using TikTokClone.Infrastructure.Authentication;
using TikTokClone.Infrastructure.Data;

namespace TikTokClone.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signinManager;
        private readonly ITokenService _tokenService;
        private readonly AppDbContext _context;
        private readonly JwtSettings _jwtSettings;

        public AuthService(
            UserManager<User> userManager,
            SignInManager<User> signinManager,
            ITokenService tokenService,
            AppDbContext context,
            JwtSettings jwtSettings
        )
        {
            _userManager = userManager;
            _signinManager = signinManager;
            _tokenService = tokenService;
            _context = context;
            _jwtSettings = jwtSettings;
        }
        public async Task<AuthResponseDto> LoginAsync(LoginRequestDto request)
        {
            var user = await _userManager.FindByEmailAsync(request.UsernameOrEmail) ??
                await _userManager.FindByNameAsync(request.UsernameOrEmail);

            if (user == null)
            {
                return new AuthResponseDto
                {
                    IsSuccess = false,
                    Message = "Username or password is not correct",
                    ErrorCode = "INVALID_CREDENTIALS"
                };
            }

            var result = await _signinManager.CheckPasswordSignInAsync(
                user, request.Password, true);

            if (!result.Succeeded)
            {
                if (result.IsLockedOut)
                {
                    return new AuthResponseDto
                    {
                        IsSuccess = false,
                        Message = "You account is logging failed too much time",
                        ErrorCode = "ACCOUNT_LOCKED"
                    };
                }

                return new AuthResponseDto
                {
                    IsSuccess = false,
                    Message = "Username or password is not correct",
                    ErrorCode = "INVALID_CREDENTIALS"
                };
            }

            if (!user.EmailConfirmed)
            {
                return new AuthResponseDto
                {
                    IsSuccess = false,
                    Message = "Email is not confirmed",
                    ErrorCode = "EMAIL_NOT_CONFIRMED"
                };
            }

            user.RecordLogin();
            await _userManager.UpdateAsync(user);

            var token = _tokenService.GenerateToken(user);
            var refreshToken = _tokenService.GenerateRefreshToken();

            var refreshTokenEntity = new RefreshToken
            {
                Token = token,
                UserId = user.Id,
                ExpiresAt = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpirationInDays),
                CreatedAt = DateTime.UtcNow
            };

            _context.RefreshTokens.Add(refreshTokenEntity);
            await _context.SaveChangesAsync();

            return new AuthResponseDto
            {
                IsSuccess = true,
                Message = "Login successfully",
                Token = token,
                RefreshToken = refreshToken,
                ExpiresAt = DateTime.UtcNow
                    .AddMinutes(_jwtSettings.ExpirationInMinutes),
            };
        }

        public Task<AuthResponseDto> RegisterAsync(RegisterRequestDto request)
        {
            throw new NotImplementedException();
        }

        public Task<AuthResponseDto> RefreshTokenAsync(string refreshToken)
        {
            throw new NotImplementedException();
        }

        public Task<bool> LogoutAsync(string userId)
        {
            throw new NotImplementedException();
        }

        public Task<AuthResponseDto> ConfirmEmailAsync(string userId, string token)
        {
            throw new NotImplementedException();
        }

        public Task<bool> SendEmailConfirmationAsync(string email)
        {
            throw new NotImplementedException();
        }
    }
}
