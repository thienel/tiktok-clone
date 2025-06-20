
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Identity;
using TikTokClone.Application.Constants;
using TikTokClone.Application.DTOs;
using TikTokClone.Application.Exceptions;
using TikTokClone.Application.Interfaces.Repositories;
using TikTokClone.Application.Interfaces.Services;
using TikTokClone.Domain.Entities;
using TikTokClone.Domain.Exceptions;

namespace TikTokClone.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly UserManager<User> _userManager;
        public UserService(IUserRepository userRepository, UserManager<User> userManager)
        {
            _userRepository = userRepository;
            _userManager = userManager;
        }

        public async Task<UserResponseDto> GetProfileAsync(string userId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(userId))
                {
                    return new UserResponseDto
                    {
                        IsSuccess = false,
                        Message = "User ID cannot be null or empty",
                        ErrorCode = ErrorCodes.INVALID_CREDENTIALS
                    };
                }

                userId = userId.Trim();

                var user = await _userRepository.GetByIdAsync(userId);
                if (user == null)
                {
                    return new UserResponseDto
                    {
                        IsSuccess = false,
                        Message = "User not found",
                        ErrorCode = ErrorCodes.USER_NOT_FOUND
                    };
                }

                return new UserResponseDto
                {
                    IsSuccess = true,
                    Message = "User profile retrieved successfully",
                    Data = new ProfileResponseDto
                    {
                        Username = user.UserName!,
                        Name = user.Name,
                        AvatarURL = user.AvatarURL,
                        IsVerified = user.IsVerified,
                        Bio = user.Bio
                    }
                };
            }
            catch (Exception ex)
            {
                var (errorCode, message) = ExceptionHandler.HandleGenericException(ex);
                return new UserResponseDto
                {
                    IsSuccess = false,
                    Message = message,
                    ErrorCode = errorCode
                };
            }

        }

        public async Task<UserResponseDto> ChangeAvatarAsync(string userId, string avatarURL)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(userId))
                {
                    return new UserResponseDto
                    {
                        IsSuccess = false,
                        Message = "User ID cannot be null or empty",
                        ErrorCode = ErrorCodes.INVALID_CREDENTIALS
                    };
                }

                if (!string.IsNullOrWhiteSpace(avatarURL) && !Uri.IsWellFormedUriString(avatarURL, UriKind.Absolute))
                {
                    return new UserResponseDto
                    {
                        IsSuccess = false,
                        Message = "Invalid avatar URL format",
                        ErrorCode = ErrorCodes.INVALID_AVATAR_URL
                    };
                }

                userId = userId.Trim();

                var user = await _userRepository.GetByIdAsync(userId);
                if (user == null)
                {
                    return new UserResponseDto
                    {
                        IsSuccess = false,
                        Message = "User not found",
                        ErrorCode = ErrorCodes.USER_NOT_FOUND
                    };
                }

                if (!user.ChangeAvatar(avatarURL))
                {
                    return new UserResponseDto
                    {
                        IsSuccess = false,
                        Message = "Failed to change avatar",
                        ErrorCode = ErrorCodes.USER_UPDATE_FAILED
                    };
                }

                _userRepository.Update(user);
                await _userRepository.SaveChangesAsync();

                return new UserResponseDto
                {
                    IsSuccess = true,
                    Message = "Avatar changed successfully",
                };
            }
            catch (DomainException domainEx)
            {
                var (errorCode, message) = ExceptionHandler.HandleDomainException(domainEx);
                return new UserResponseDto
                {
                    IsSuccess = false,
                    Message = message,
                    ErrorCode = errorCode
                };
            }
            catch (Exception ex)
            {
                var (errorCode, message) = ExceptionHandler.HandleGenericException(ex);
                return new UserResponseDto
                {
                    IsSuccess = false,
                    Message = message,
                    ErrorCode = errorCode
                };
            }
        }

        public async Task<UserResponseDto> ChangeBioAsync(string userId, string bio)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(userId))
                {
                    return new UserResponseDto
                    {
                        IsSuccess = false,
                        Message = "User ID cannot be null or empty",
                        ErrorCode = ErrorCodes.INVALID_CREDENTIALS
                    };
                }

                userId = userId.Trim();

                var user = await _userRepository.GetByIdAsync(userId);
                if (user == null)
                {
                    return new UserResponseDto
                    {
                        IsSuccess = false,
                        Message = "User not found",
                        ErrorCode = ErrorCodes.USER_NOT_FOUND
                    };
                }

                if (!user.ChangeBio(bio))
                {
                    return new UserResponseDto
                    {
                        IsSuccess = false,
                        Message = "Failed to change bio",
                        ErrorCode = ErrorCodes.USER_UPDATE_FAILED
                    };
                }

                return new UserResponseDto
                {
                    IsSuccess = true,
                    Message = "Bio changed successfully",
                };
            }
            catch (DomainException domainEx)
            {
                var (errorCode, message) = ExceptionHandler.HandleDomainException(domainEx);
                return new UserResponseDto
                {
                    IsSuccess = false,
                    Message = message,
                    ErrorCode = errorCode
                };
            }
            catch (Exception ex)
            {
                var (errorCode, message) = ExceptionHandler.HandleGenericException(ex);
                return new UserResponseDto
                {
                    IsSuccess = false,
                    Message = message,
                    ErrorCode = errorCode
                };
            }
        }

        public async Task<UserResponseDto> ChangeNameAsync(string userId, string name)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(userId))
                {
                    return new UserResponseDto
                    {
                        IsSuccess = false,
                        Message = "User ID cannot be null or empty",
                        ErrorCode = ErrorCodes.INVALID_CREDENTIALS
                    };
                }

                userId = userId.Trim();

                var user = await _userRepository.GetByIdAsync(userId);
                if (user == null)
                {
                    return new UserResponseDto
                    {
                        IsSuccess = false,
                        Message = "User not found",
                        ErrorCode = ErrorCodes.USER_NOT_FOUND
                    };
                }

                if (!user.ChangeName(name))
                {
                    return new UserResponseDto
                    {
                        IsSuccess = false,
                        Message = "Failed to change name",
                        ErrorCode = ErrorCodes.USER_UPDATE_FAILED
                    };
                }

                return new UserResponseDto
                {
                    IsSuccess = true,
                    Message = "Name changed successfully",
                };
            }
            catch (DomainException domainEx)
            {
                var (errorCode, message) = ExceptionHandler.HandleDomainException(domainEx);
                return new UserResponseDto
                {
                    IsSuccess = false,
                    Message = message,
                    ErrorCode = errorCode
                };
            }
            catch (Exception ex)
            {
                var (errorCode, message) = ExceptionHandler.HandleGenericException(ex);
                return new UserResponseDto
                {
                    IsSuccess = false,
                    Message = message,
                    ErrorCode = errorCode
                };
            }
        }

        public async Task<UserResponseDto> ChangeUsernameByIdAsync(string userId, string username)
        {
            if (string.IsNullOrWhiteSpace(userId))
            {
                return new UserResponseDto
                {
                    IsSuccess = false,
                    Message = "User Id can not be null or empty",
                    ErrorCode = ErrorCodes.INVALID_CREDENTIALS
                };
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return new UserResponseDto
                {
                    IsSuccess = false,
                    Message = "User not found",
                    ErrorCode = ErrorCodes.USER_NOT_FOUND
                };
            }

            if (!user.ChangeUserName(username))
            {
                return new UserResponseDto
                {
                    IsSuccess = false,
                    Message = "Change username failed",
                    ErrorCode = ErrorCodes.USERNAME_CHANGE_FAILED
                };
            }

            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {

                return new UserResponseDto
                {
                    IsSuccess = false,
                    Message = "Change username failed",
                    ErrorCode = ErrorCodes.USERNAME_CHANGE_FAILED
                };
            }

            return new UserResponseDto
            {
                IsSuccess = true,
                Message = "Change username successfully",
            };
        }

        public async Task<UserResponseDto> ChangeUsernameByEmailAsync(string email, string username)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return new UserResponseDto
                {
                    IsSuccess = false,
                    Message = "Email can not be null or empty",
                    ErrorCode = ErrorCodes.INVALID_CREDENTIALS
                };
            }

            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return new UserResponseDto
                {
                    IsSuccess = false,
                    Message = "User not found",
                    ErrorCode = ErrorCodes.USER_NOT_FOUND
                };
            }

            if (!user.ChangeUserName(username))
            {
                return new UserResponseDto
                {
                    IsSuccess = false,
                    Message = "Change username failed",
                    ErrorCode = ErrorCodes.USERNAME_CHANGE_FAILED
                };
            }

            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {

                return new UserResponseDto
                {
                    IsSuccess = false,
                    Message = "Change username failed",
                    ErrorCode = ErrorCodes.USERNAME_CHANGE_FAILED
                };
            }

            return new UserResponseDto
            {
                IsSuccess = true,
                Message = "Change username successfully",
            };
        }

        public async Task<UserResponseDto> VerifyUserAsync(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
            {
                return new UserResponseDto
                {
                    IsSuccess = false,
                    Message = "User Id can not be null or empty",
                    ErrorCode = ErrorCodes.INVALID_CREDENTIALS
                };
            }

            var user = await _userRepository.GetByIdAsync(userId.Trim());
            if (user == null)
            {
                return new UserResponseDto
                {
                    IsSuccess = false,
                    Message = "User not found",
                    ErrorCode = ErrorCodes.USER_NOT_FOUND
                };
            }

            try
            {
                user.Verify();
                _userRepository.Update(user);
                await _userRepository.SaveChangesAsync();

                return new UserResponseDto
                {
                    IsSuccess = true,
                    Message = "Verified user successfully"
                };
            }
            catch (DomainException ex)
            {
                var (errorCode, message) = ExceptionHandler.HandleDomainException(ex);
                return new UserResponseDto
                {
                    IsSuccess = false,
                    Message = message,
                    ErrorCode = errorCode
                };
            }
            catch (Exception ex)
            {
                var (errorCode, message) = ExceptionHandler.HandleGenericException(ex);
                return new UserResponseDto
                {
                    IsSuccess = false,
                    Message = message,
                    ErrorCode = errorCode
                };
            }
        }

        public async Task<UserResponseDto> UnVerifyUserAsync(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
            {
                return new UserResponseDto
                {
                    IsSuccess = false,
                    Message = "User Id can not be null or empty",
                    ErrorCode = ErrorCodes.INVALID_CREDENTIALS
                };
            }

            var user = await _userRepository.GetByIdAsync(userId.Trim());
            if (user == null)
            {
                return new UserResponseDto
                {
                    IsSuccess = false,
                    Message = "User not found",
                    ErrorCode = ErrorCodes.USER_NOT_FOUND
                };
            }

            try
            {
                user.UnVerify();
                _userRepository.Update(user);
                await _userRepository.SaveChangesAsync();

                return new UserResponseDto
                {
                    IsSuccess = true,
                    Message = "Unverified user successfully"
                };
            }
            catch (DomainException ex)
            {
                var (errorCode, message) = ExceptionHandler.HandleDomainException(ex);
                return new UserResponseDto
                {
                    IsSuccess = false,
                    Message = message,
                    ErrorCode = errorCode
                };
            }
            catch (Exception ex)
            {
                var (errorCode, message) = ExceptionHandler.HandleGenericException(ex);
                return new UserResponseDto
                {
                    IsSuccess = false,
                    Message = message,
                    ErrorCode = errorCode
                };
            }
        }

        public async Task<SearchUserResponseDto> Search(string value, int? limit = 10)
        {
            var response = new SearchUserResponseDto();
            if (string.IsNullOrWhiteSpace(value))
            {
                return response;
            }

            value = value.Trim().ToLower();
            var users = await _userRepository
                .GetManyAsync(u =>
                    u.UserName!.ToLower().Contains(value) ||
                    u.Name.ToLower().Contains(value));

            users = users.Take(limit ?? 10);

            response.Users = users.Select(u => new ProfileResponseDto
            {
                Username = u.UserName!,
                Name = u.Name,
                AvatarURL = u.AvatarURL,
                IsVerified = u.IsVerified,
                Bio = u.Bio
            }).ToList();

            return response;
        }

        public UserResponseDto CheckValidBirthDate(DateOnly birthDate)
        {
            var result = User.IsValidBirthDate(birthDate);

            if (!result)
            {
                return new UserResponseDto
                {
                    IsSuccess = false,
                    Message = "Birthdate is not valid",
                    ErrorCode = ErrorCodes.INVALID_BIRTH_DATE
                };
            }

            return new UserResponseDto
            {
                IsSuccess = true,
                Message = "Birthdate is valid"
            };
        }

        public async Task<UserResponseDto> CheckValidUsernameAsync(string username)
        {

            if (string.IsNullOrWhiteSpace(username))
            {
                return new UserResponseDto
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
                return new UserResponseDto
                {
                    IsSuccess = false,
                    Message = "Invalid username format",
                    ErrorCode = ErrorCodes.INVALID_USERNAME_FORMAT
                };
            }

            var existingUser = await _userManager.FindByNameAsync(username);
            if (existingUser != null)
            {
                return new UserResponseDto
                {
                    IsSuccess = false,
                    Message = "Username is already in use",
                    ErrorCode = ErrorCodes.USERNAME_USED
                };
            }

            return new UserResponseDto
            {
                IsSuccess = true,
                Message = "Username is valid"
            };
        }

    }
}
