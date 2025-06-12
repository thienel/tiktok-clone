using TikTokClone.Application.DTOs;
using TikTokClone.Domain.Entities;

namespace TikTokClone.Application.Interfaces.Services
{
    public interface IUserService
    {
        Task<ProfileResponseDto> GetProfileAsync(string userId);
        Task<ProfileResponseDto> UpdateProfileAsync(User user);
        Task ChangeAvatarAsync(string userId, string avatarURL);
        // Task GetNotifications();
        Task Search(string value);
    }
}
