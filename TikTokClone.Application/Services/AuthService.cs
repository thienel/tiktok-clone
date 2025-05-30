using Microsoft.AspNetCore.Identity;
using TikTokClone.Application.DTOs;
using TikTokClone.Application.Interfaces.Repositories;
using TikTokClone.Application.Interfaces.Services;
using TikTokClone.Application.Interfaces.Settings;
using TikTokClone.Application.Constants;
using TikTokClone.Application.Exceptions;
using TikTokClone.Domain.Entities;
using TikTokClone.Domain.Exceptions;

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
            try
            {
                var user = await _userManager.FindByEmailAsync(request.UsernameOrEmail) ??
                    await _userManager.FindByNameAsync(request.UsernameOrEmail);

                if (user == null)
                {
                    return new AuthResponseDto
                    {
                        IsSuccess = false,
                        Message = "Username or email is not correct",
                        ErrorCode = ErrorCodes.INVALID_CREDENTIALS
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
                            ErrorCode = ErrorCodes.ACCOUNT_LOCKED
                        };
                    }

                    return new AuthResponseDto
                    {
                        IsSuccess = false,
                        Message = "Username or password is not correct",
                        ErrorCode = ErrorCodes.INVALID_CREDENTIALS
                    };
                }

                if (!user.EmailConfirmed)
                {
                    return new AuthResponseDto
                    {
                        IsSuccess = false,
                        Message = "Your email address has not been confirmed",
                        ErrorCode = ErrorCodes.EMAIL_NOT_CONFIRMED
                    };
                }

                user.RecordLogin();
                var updateResult = await _userManager.UpdateAsync(user);

                if (!updateResult.Succeeded)
                {
                    return new AuthResponseDto
                    {
                        IsSuccess = false,
                        Message = "Failed to update user login information",
                        ErrorCode = ErrorCodes.USER_UPDATE_FAILED
                    };
                }

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
            catch (Exception ex)
            {
                var (errorCode, message) = ExceptionHandler.HandleGenericException(ex);
                return new AuthResponseDto
                {
                    IsSuccess = false,
                    Message = message,
                    ErrorCode = errorCode
                };
            }
        }

        public async Task<AuthResponseDto> RegisterAsync(RegisterRequestDto request)
        {
            try
            {
                var existingUser = await _userManager.FindByEmailAsync(request.Email);
                if (existingUser != null)
                {
                    return new AuthResponseDto
                    {
                        IsSuccess = false,
                        Message = "This email is already in use",
                        ErrorCode = ErrorCodes.EMAIL_USED
                    };
                }

                var userName = await GenerateUniqueUsernameAsync();
                var user = new User(request.Email, userName, request.BirthDate, userName);

                var result = await _userManager.CreateAsync(user, request.Password);
                if (!result.Succeeded)
                {
                    return new AuthResponseDto
                    {
                        IsSuccess = false,
                        Message = string.Join(", ", result.Errors.Select(e => e.Description)),
                        ErrorCode = ErrorCodes.REGISTRATION_FAILED
                    };
                }

                var emailToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);

                // TODO: Send Email confirmation

                return new AuthResponseDto
                {
                    IsSuccess = true,
                    Message = "Registered successfully"
                };
            }
            catch (DomainException domainEx)
            {
                var (errorCode, message) = ExceptionHandler.HandleDomainException(domainEx);
                return new AuthResponseDto
                {
                    IsSuccess = false,
                    Message = message,
                    ErrorCode = errorCode
                };
            }
            catch (Exception ex)
            {
                var (errorCode, message) = ExceptionHandler.HandleGenericException(ex);
                return new AuthResponseDto
                {
                    IsSuccess = false,
                    Message = message,
                    ErrorCode = errorCode
                };
            }
        }

        public async Task<AuthResponseDto> RefreshTokenAsync(string refreshToken)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(refreshToken))
                {
                    return new AuthResponseDto
                    {
                        IsSuccess = false,
                        Message = "Refresh token is required",
                        ErrorCode = ErrorCodes.INVALID_REFRESH_TOKEN
                    };
                }

                var tokenEntity = await _refreshTokenRepo.GetByTokenAsync(refreshToken);
                if (tokenEntity == null || !tokenEntity.CanBeRefreshed())
                {
                    return new AuthResponseDto
                    {
                        IsSuccess = false,
                        Message = "Invalid or expired refresh token",
                        ErrorCode = ErrorCodes.INVALID_REFRESH_TOKEN
                    };
                }

                var user = await _userManager.FindByIdAsync(tokenEntity.UserId);
                if (user == null)
                {
                    return new AuthResponseDto
                    {
                        IsSuccess = false,
                        Message = "User not found",
                        ErrorCode = ErrorCodes.USER_NOT_FOUND
                    };
                }

                // Generate new tokens first
                var newToken = _tokenService.GenerateToken(user);
                var newRefreshToken = _tokenService.GenerateRefreshToken();

                // Revoke old refresh token
                tokenEntity.Revoke(newRefreshToken);
                _refreshTokenRepo.Update(tokenEntity);

                // Create new refresh token
                var newRefreshTokenEntity = new RefreshToken
                {
                    Token = newRefreshToken,
                    UserId = user.Id,
                    ExpiresAt = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpirationInDays),
                    CreatedAt = DateTime.UtcNow
                };

                await _refreshTokenRepo.AddAsync(newRefreshTokenEntity);
                await _refreshTokenRepo.SaveChangesAsync();

                return new AuthResponseDto
                {
                    IsSuccess = true,
                    Message = "Tokens refreshed successfully",
                    Token = newToken,
                    RefreshToken = newRefreshToken,
                    ExpiresAt = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpirationInMinutes)
                };
            }
            catch (Exception ex)
            {
                var (errorCode, message) = ExceptionHandler.HandleGenericException(ex);
                return new AuthResponseDto
                {
                    IsSuccess = false,
                    Message = message,
                    ErrorCode = errorCode
                };
            }
        }
        public async Task<bool> LogoutAsync(string userId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(userId))
                    return false;

                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                    return false;

                // Get only active tokens to revoke
                var activeTokens = await _refreshTokenRepo.GetActiveByUserIdAsync(userId);
                foreach (var token in activeTokens)
                {
                    token.Revoke();
                    _refreshTokenRepo.Update(token);
                }

                await _refreshTokenRepo.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<AuthResponseDto> ConfirmEmailAsync(string userId, string token)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(token))
                {
                    return new AuthResponseDto
                    {
                        IsSuccess = false,
                        Message = "User ID and token are required",
                        ErrorCode = ErrorCodes.VALIDATION_ERROR
                    };
                }

                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    return new AuthResponseDto
                    {
                        IsSuccess = false,
                        Message = "User not found",
                        ErrorCode = ErrorCodes.USER_NOT_FOUND
                    };
                }

                var result = await _userManager.ConfirmEmailAsync(user, token);
                if (!result.Succeeded)
                {
                    return new AuthResponseDto
                    {
                        IsSuccess = false,
                        Message = "Failed to confirm email",
                        ErrorCode = ErrorCodes.EMAIL_CONFIRMATION_FAILED
                    };
                }

                // Call domain method to trigger domain events
                user.ConfirmEmail();
                await _userManager.UpdateAsync(user);

                return new AuthResponseDto
                {
                    IsSuccess = true,
                    Message = "Email confirmed successfully"
                };
            }
            catch (Exception ex)
            {
                var (errorCode, message) = ExceptionHandler.HandleGenericException(ex);
                return new AuthResponseDto
                {
                    IsSuccess = false,
                    Message = message,
                    ErrorCode = errorCode
                };
            }
        }

        public async Task<bool> SendEmailConfirmationAsync(string email)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(email))
                    return false;

                var user = await _userManager.FindByEmailAsync(email);
                if (user == null || user.EmailConfirmed)
                    return false;

                var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

                // TODO: Send email with confirmation link
                // await _emailService.SendEmailConfirmationAsync(user.Email, user.Id, token);

                return true;
            }
            catch
            {
                return false;
            }
        }

        private async Task<string> GenerateUniqueUsernameAsync()
        {
            var userName = "user" + Guid.NewGuid().ToString("N")[..12];
            var existingUser = await _userManager.FindByNameAsync(userName);

            if (existingUser == null)
                return userName;

            return await GenerateUniqueUsernameAsync();
        }
    }
}
