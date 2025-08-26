
namespace AuthService.DTOs
{
    public class ProfileResponseDto
    {
        public string Username { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? AvatarURL { get; set; }
        public bool IsVerified { get; set; } = false;
        public string? Bio { get; set; }
    }

    public class UserResponseDto : BaseResponseDto<ProfileResponseDto>
    {
    }

    public class SearchUserResponseDto
    {
        public IEnumerable<ProfileResponseDto> Users { get; set; } = new List<ProfileResponseDto>();
    }

    public class CheckUsernameDto
    {
        public string Username { get; set; } = string.Empty;
    }

    public class ChangeUsernameDto
    {
        public string IdOrEmail { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
    }

    public class CheckBirthdateDto
    {
        public DateOnly BirthDate { get; set; }
    }

    public class SearchUserDto
    {
        public string Value { get; set; } = string.Empty;
        public int Limit { get; set; } = 10;
    }
}
