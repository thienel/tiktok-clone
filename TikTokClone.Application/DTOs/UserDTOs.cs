namespace TikTokClone.Application.DTOs
{
    public class UserReponseDto
    {
        public string Name { get; set; } = string.Empty;
        public string? AvatarURL { get; set; }
        public bool IsVerified { get; set; } = false;
        public string? Bio { get; set; }
    }
}
