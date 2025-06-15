
namespace TikTokClone.Application.DTOs
{
    public class ProfileResponseDto
    {
        public string Name { get; set; } = string.Empty;
        public string? AvatarURL { get; set; }
        public bool IsVerified { get; set; } = false;
        public string? Bio { get; set; }
    }

    public class UserResponseDto
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; } = string.Empty;
        public string ErrorCode { get; set; } = string.Empty;
        public ProfileResponseDto? Profile { get; set; } = null;
    }

    public class SearchUserResponseDto
    {
        List<ProfileResponseDto> Users { get; set; } = new List<ProfileResponseDto>();
    }
}
