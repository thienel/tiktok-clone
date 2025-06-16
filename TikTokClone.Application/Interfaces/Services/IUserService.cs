using TikTokClone.Application.DTOs;

namespace TikTokClone.Application.Interfaces.Services
{
    public interface IUserService
    {
        Task<UserResponseDto> GetProfileAsync(string userId);
        Task<UserResponseDto> ChangeAvatarAsync(string userId, string avatarURL);
        Task<UserResponseDto> ChangeBioAsync(string userId, string bio);
        Task<UserResponseDto> ChangeNameAsync(string userId, string name);
        Task<UserResponseDto> ChangeUsernameByIdAsync(string userId, string username);
        Task<UserResponseDto> ChangeUsernameByEmailAsync(string email, string username);
        Task<UserResponseDto> VerifyUserAsync(string userId);
        Task<UserResponseDto> UnVerifyUserAsync(string userId);
        Task<SearchUserResponseDto> Search(string value);
        Task<UserResponseDto> CheckValidUsernameAsync(string username);
        UserResponseDto CheckValidBirthDate(DateOnly birthDate);
    }
}
