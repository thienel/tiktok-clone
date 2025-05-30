using Microsoft.AspNetCore.Identity;
using TikTokClone.Application.DTOs;
using TikTokClone.Application.Interfaces.Repositories;
using TikTokClone.Application.Interfaces.Services;
using TikTokClone.Application.Interfaces.Settings;
using TikTokClone.Domain.Entities;

namespace TikTokClone.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signinManager;
        private readonly ITokenService _tokenService;
        private readonly IRefreshTokenRepository _refreshTokenRepo;
        private readonly IJwtSettings _jwtSettings;

        public AuthService(
            UserManager<User> userManager,
            SignInManager<User> signinManager,
            ITokenService tokenService,
            IRefreshTokenRepository refreshTokenRepo,
            IJwtSettings jwtSettings
        )
        {
            _userManager = userManager;
            _signinManager = signinManager;
            _tokenService = tokenService;
            _refreshTokenRepo = refreshTokenRepo;
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
                    Message = "Username or email is not correct",
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
                        Message = "Your account has been locked due to too many failed login attempts",
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
                    Message = "Your email address has not been confirmed",
                    ErrorCode = "EMAIL_NOT_CONFIRMED"
                };
            }

            user.RecordLogin();
            await _userManager.UpdateAsync(user);

            var token = _tokenService.GenerateToken(user);
            var refreshToken = _tokenService.GenerateRefreshToken();

            var refreshTokenEntity = new RefreshToken
            {
                Token = refreshToken,
                UserId = user.Id,
                ExpiresAt = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpirationInDays),
                CreatedAt = DateTime.UtcNow
            };

            await _refreshTokenRepo.AddAsync(refreshTokenEntity);
            await _refreshTokenRepo.SaveChangesAsync();

            return new AuthResponseDto
            {
                IsSuccess = true,
                Message = "Logged in successfully",
                Token = token,
                RefreshToken = refreshToken,
                ExpiresAt = DateTime.UtcNow
                    .AddMinutes(_jwtSettings.ExpirationInMinutes),
            };
        }

        public async Task<AuthResponseDto> RegisterAsync(RegisterRequestDto request)
        {
            var existingUser = await _userManager.FindByEmailAsync(request.Email);
            if (existingUser != null)
            {
                return new AuthResponseDto
                {
                    IsSuccess = false,
                    Message = "This email is already in use",
                    ErrorCode = "EMAIL_USED"
                };
            }

            try
            {
                var userName = await GenerateUniqueUsernameAsync();
                var user = new User(request.Email, userName, request.BirthDate, userName);

                var result = await _userManager.CreateAsync(user, request.Password);
                if (!result.Succeeded)
                {
                    return new AuthResponseDto
                    {
                        IsSuccess = false,
                        Message = string.Join(", ", result.Errors.Select(e => e.Description))
                    };
                }

                var emailToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);

                /**

                    Send Email

                **/

                return new AuthResponseDto
                {
                    IsSuccess = true,
                    Message = "Registered successfully"
                };
            }
            catch (Exception)
            {
                return new AuthResponseDto
                {
                    IsSuccess = false,
                    Message = "An unexpected error occurred"
                };
            }
        }

        private async Task<string> GenerateUniqueUsernameAsync()
        {
            var userName = "user" + Guid.NewGuid().ToString("N").Substring(0, 12);
            if ((await _userManager.FindByNameAsync(userName)) == null)
            {
                return userName;
            }

            return await GenerateUniqueUsernameAsync();
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
