
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
        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
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
                        Message = "User ID cannot be null or empty.",
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
                        Message = "User not found.",
                        ErrorCode = ErrorCodes.USER_NOT_FOUND
                    };
                }

                return new UserResponseDto
                {
                    IsSuccess = true,
                    Message = "User profile retrieved successfully.",
                    Profile = new ProfileResponseDto
                    {
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
                        Message = "User ID cannot be null or empty.",
                        ErrorCode = ErrorCodes.INVALID_CREDENTIALS
                    };
                }

                if (!string.IsNullOrWhiteSpace(avatarURL) && !Uri.IsWellFormedUriString(avatarURL, UriKind.Absolute))
                {
                    return new UserResponseDto
                    {
                        IsSuccess = false,
                        Message = "Invalid avatar URL format.",
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
                        Message = "User not found.",
                        ErrorCode = ErrorCodes.USER_NOT_FOUND
                    };
                }

                if (!user.ChangeAvatar(avatarURL))
                {
                    return new UserResponseDto
                    {
                        IsSuccess = false,
                        Message = "Failed to change avatar.",
                        ErrorCode = ErrorCodes.USER_UPDATE_FAILED
                    };
                }

                _userRepository.Update(user);
                await _userRepository.SaveChangesAsync();

                return new UserResponseDto
                {
                    IsSuccess = true,
                    Message = "Avatar changed successfully.",
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

        public Task<UserResponseDto> ChangeBioAsync(string userId, string bio)
        {
            throw new NotImplementedException();
        }

        public Task<UserResponseDto> ChangeNameAsync(string userId, string name)
        {
            throw new NotImplementedException();
        }

        public Task<UserResponseDto> ChangeUserNameAsync(string userId, string userName)
        {
            throw new NotImplementedException();
        }

        public Task<UserResponseDto> VerifyUserAsync(string userId)
        {
            throw new NotImplementedException();
        }

        public Task<UserResponseDto> UnVerifyUserAsync(string userId)
        {
            throw new NotImplementedException();
        }

        public Task<SearchUserResponseDto> Search(string value)
        {
            throw new NotImplementedException();
        }

    }
}
