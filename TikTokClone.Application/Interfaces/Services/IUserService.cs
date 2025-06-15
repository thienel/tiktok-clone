using TikTokClone.Application.DTOs;
using TikTokClone.Domain.Entities;

namespace TikTokClone.Application.Interfaces.Services
{
    public interface IUserService
    {
        Task<UserResponseDto> GetProfileAsync(string userId);
        Task<UserResponseDto> ChangeAvatarAsync(string userId, string avatarURL);
        Task<UserResponseDto> ChangeBioAsync(string userId, string bio);
        Task<UserResponseDto> ChangeNameAsync(string userId, string name);
        Task<UserResponseDto> ChangeUserNameAsync(string userId, string userName);
        Task<UserResponseDto> VerifyUserAsync(string userId);
        Task<UserResponseDto> UnVerifyUserAsync(string userId);
        Task<SearchUserResponseDto> Search(string value);
    }
}
