using Microsoft.AspNetCore.Identity;
using TikTokClone.Application.DTOs;
using TikTokClone.Application.Interfaces.Repositories;
using TikTokClone.Application.Interfaces.Services;
using TikTokClone.Application.Interfaces.Settings;
using TikTokClone.Application.Constants;
using TikTokClone.Application.Exceptions;
using TikTokClone.Domain.Entities;
using TikTokClone.Domain.Exceptions;
using System.Text.RegularExpressions;

namespace TikTokClone.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signinManager;
        private readonly ITokenService _tokenService;
        private readonly IRefreshTokenRepository _refreshTokenRepo;
        private readonly IJwtSettings _jwtSettings;
        private readonly IEmailService _emailService;
        private readonly IEmailVerificationRepository _emailVerificationRepo;

        public AuthService(
            UserManager<User> userManager,
            SignInManager<User> signinManager,
            ITokenService tokenService,
            IRefreshTokenRepository refreshTokenRepo,
            IJwtSettings jwtSettings,
            IEmailService emailServive,
            IEmailVerificationRepository emailVerificationRepo
        )
        {
            _userManager = userManager;
            _signinManager = signinManager;
            _tokenService = tokenService;
            _refreshTokenRepo = refreshTokenRepo;
            _jwtSettings = jwtSettings;
            _emailService = emailServive;
            _emailVerificationRepo = emailVerificationRepo;
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

                    if (!user.EmailConfirmed)
                    {
                        return new AuthResponseDto
                        {
                            IsSuccess = false,
                            Message = "Your email address has not been confirmed",
                            ErrorCode = ErrorCodes.EMAIL_NOT_CONFIRMED
                        };
                    }

                    return new AuthResponseDto
                    {
                        IsSuccess = false,
                        Message = "Username or password is not correct",
                        ErrorCode = ErrorCodes.INVALID_CREDENTIALS
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

                var userResponse = new UserReponseDto
                {
                    Name = user.Name,
                    AvatarURL = user.AvatarURL,
                    IsVerified = user.IsVerified,
                    Bio = user.Bio
                };

                return new AuthResponseDto
                {
                    IsSuccess = true,
                    Message = "Logged in successfully",
                    Token = token,
                    RefreshToken = refreshToken,
                    ExpiresAt = DateTime.UtcNow
                        .AddMinutes(_jwtSettings.ExpirationInMinutes),
                    User = userResponse
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

                var emailVerification = await _emailVerificationRepo.FindByEmailAsync(request.Email);
                if (emailVerification == null)
                {
                    return new AuthResponseDto
                    {
                        IsSuccess = false,
                        Message = "Verification code is not found",
                        ErrorCode = ErrorCodes.VERIFICATION_CODE_NOT_FOUND
                    };
                }

                if (!emailVerification.IsVertificationCodeActive())
                {
                    return new AuthResponseDto
                    {
                        IsSuccess = false,
                        Message = "Verification code is expired",
                        ErrorCode = ErrorCodes.VERIFICATION_CODE_EXPIRED
                    };
                }

                if (emailVerification.Code != request.VerificationCode.Trim())
                {
                    return new AuthResponseDto
                    {
                        IsSuccess = false,
                        Message = "Verification code is not match",
                        ErrorCode = ErrorCodes.INVALID_VERIFICATION_CODE
                    };
                }

                var userName = await GenerateUniqueUsernameAsync();
                var user = new User(request.Email, request.BirthDate, userName);
                user.ConfirmEmail();

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

                var newToken = _tokenService.GenerateToken(user);
                var newRefreshToken = _tokenService.GenerateRefreshToken();

                tokenEntity.Revoke(newRefreshToken);
                _refreshTokenRepo.Update(tokenEntity);

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
        public async Task<AuthResponseDto> LogoutAsync(string userId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(userId))
                {
                    return new AuthResponseDto
                    {
                        IsSuccess = false,
                        Message = "User id is not valid",
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

                var activeTokens = await _refreshTokenRepo.GetActiveByUserIdAsync(userId);
                foreach (var token in activeTokens)
                {
                    token.Revoke();
                    _refreshTokenRepo.Update(token);
                }

                await _refreshTokenRepo.SaveChangesAsync();
                return new AuthResponseDto
                {
                    IsSuccess = true,
                    Message = "Logged out successfully",
                };
            }
            catch
            {
                return new AuthResponseDto
                {
                    IsSuccess = false,
                    Message = "An internal server error occurred",
                    ErrorCode = ErrorCodes.UNEXPECTED_ERROR
                };
            }
        }

        private async Task<string> GenerateUniqueUsernameAsync()
        {
            var rd = new Random();
            while (true)
            {
                var userName = "user";
                userName += rd.Next(0, 1_000_000).ToString("D6");
                userName += rd.Next(0, 1_000_000).ToString("D6");

                var existingUser = await _userManager.FindByNameAsync(userName);

                if (existingUser == null)
                    return userName;
            }
        }

        public async Task<AuthResponseDto> CheckValidUsername(string username)
        {

            if (string.IsNullOrWhiteSpace(username))
            {
                return new AuthResponseDto
                {
                    IsSuccess = false,
                    Message = "Username can not be empty",
                    ErrorCode = ErrorCodes.INVALID_CREDENTIALS
                };
            }

            Regex _userNameRegex = new(@"^[a-z0-9._]{2,24}$", RegexOptions.Compiled);
            username = username.Trim();
            if (!_userNameRegex.IsMatch(username))
            {
                return new AuthResponseDto
                {
                    IsSuccess = false,
                    Message = "Invalid username format",
                    ErrorCode = ErrorCodes.INVALID_USERNAME_FORMAT
                };
            }

            var existingUser = await _userManager.FindByNameAsync(username);
            if (existingUser != null)
            {
                return new AuthResponseDto
                {
                    IsSuccess = false,
                    Message = "Username is already in use",
                    ErrorCode = ErrorCodes.USERNAME_USED
                };
            }

            return new AuthResponseDto
            {
                IsSuccess = true,
                Message = "Username is valid"
            };
        }

        public async Task<AuthResponseDto> SendEmailVerificationCodeAsync(string email)
        {
            try
            {
                var existingUser = await _userManager.FindByEmailAsync(email);
                if (existingUser != null && existingUser.EmailConfirmed)
                {
                    return new AuthResponseDto
                    {
                        IsSuccess = false,
                        Message = "User's email already confirmed",
                        ErrorCode = ErrorCodes.EMAIL_ALREADY_CONFIRMED
                    };
                }

                var existingEmailVerification = await _emailVerificationRepo.FindByEmailAsync(email);
                if (existingEmailVerification != null)
                {
                    if (!existingEmailVerification.GenerateNewCode())
                    {
                        return new AuthResponseDto
                        {
                            IsSuccess = false,
                            Message = "Please wait before resend email verification",
                            ErrorCode = ErrorCodes.WAIT_BEFORE_RESEND
                        };
                    }

                    _emailVerificationRepo.Update(existingEmailVerification);
                    await _emailVerificationRepo.SaveChangesAsync();

                    if (!await _emailService.SendEmailVerificationCodeAsync(email, existingEmailVerification.Code))
                    {
                        return new AuthResponseDto
                        {
                            IsSuccess = false,
                            Message = "Fail to send email verification",
                            ErrorCode = ErrorCodes.EMAIL_SEND_FAILED
                        };
                    }

                    return new AuthResponseDto
                    {
                        IsSuccess = true,
                        Message = "Send email verification successfully"
                    };
                }

                var emailVerification = new EmailVerification(email);
                await _emailVerificationRepo.AddAsync(emailVerification);
                await _emailVerificationRepo.SaveChangesAsync();

                if (!await _emailService.SendEmailVerificationCodeAsync(email, emailVerification.Code))
                {
                    return new AuthResponseDto
                    {
                        IsSuccess = false,
                        Message = "Fail to send email verification",
                        ErrorCode = ErrorCodes.EMAIL_SEND_FAILED
                    };
                }

                return new AuthResponseDto
                {
                    IsSuccess = true,
                    Message = "Send email verification successfully"
                };

            }
            catch
            {
                return new AuthResponseDto
                {
                    IsSuccess = false,
                    Message = "An internal server error occurred",
                    ErrorCode = ErrorCodes.UNEXPECTED_ERROR
                };
            }
        }

    }
}
