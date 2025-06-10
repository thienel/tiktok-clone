
namespace TikTokClone.Application.DTOs
{
    public class ProfileResponseDto
    {
        public string Name { get; set; } = string.Empty;
        public string? AvatarURL { get; set; }
        public bool IsVerified { get; set; } = false;
        public string? Bio { get; set; }
    }

    public class UserResponseDto<T> where T : class
    {
        public string IsSuccess { get; set; }
        public string Message { get; set; }
        public string ErrorCode { get; set; }
        public T Data { get; set; }
    }
}
