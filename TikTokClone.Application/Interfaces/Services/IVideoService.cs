using System.Security;
using TikTokClone.Application.DTOs;

namespace TikTokClone.Application.Interfaces.Services
{
    public interface IVideoService
    {
        Task<VideoResponseDto> GetVideoAsync(string videoId);
        Task<VideoResponseDto> UploadVideoAsync(string userId, string videoUrl, string thumbnailUrl, string? title, string? description);
        Task<VideoResponseDto> DeleteVideoAsync(string videoId);
        Task<VideoResponseDto> ChangeVideoVisibilityAsync(string videoId, bool isVisible);
        Task<VideoResponseDto> ChangeVideoDetailsAsync(string videoId, string? title, string? description);
        Task<ListVideosResponseDto> GetHomeFeedAsync(string userId, int pageNumber = 1, int pageSize = 10);
        Task<ListVideosResponseDto> GetUserVideosAsync(string userId, int pageNumber = 1, int pageSize = 10);
        Task<ListVideosResponseDto> SearchVideosAsync(string searchTerm, int pageNumber = 1, int pageSize = 10);
    }
}
