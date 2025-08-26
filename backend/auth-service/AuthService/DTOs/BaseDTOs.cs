namespace AuthService.DTOs
{
    public class BaseResponseDto
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; } = string.Empty;
        public string? ErrorCode { get; set; }
    }

    public class BaseResponseDto<T> : BaseResponseDto where T : class
    {
        public T? Data { get; set; } = null;
    }
}
