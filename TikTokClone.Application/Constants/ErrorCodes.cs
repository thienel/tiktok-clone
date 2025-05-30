namespace TikTokClone.Application.Constants
{
    public static class ErrorCodes
    {
        // Authentication & Authorization
        public const string INVALID_CREDENTIALS = "AUTH_001";
        public const string ACCOUNT_LOCKED = "AUTH_002";
        public const string EMAIL_NOT_CONFIRMED = "AUTH_003";
        public const string EMAIL_USED = "AUTH_004";
        public const string INVALID_REFRESH_TOKEN = "AUTH_005";
        public const string TOKEN_EXPIRED = "AUTH_006";
        public const string REGISTRATION_FAILED = "AUTH_007";
        public const string EMAIL_CONFIRMATION_FAILED = "AUTH_008";
        public const string LOGOUT_FAILED = "AUTH_009";
        public const string PASSWORD_RESET_FAILED = "AUTH_010";

        // User Domain Errors (mapped from Domain layer)
        public const string INVALID_EMAIL_FORMAT = "USER_001";
        public const string INVALID_USERNAME_FORMAT = "USER_002";
        public const string INVALID_NAME_LENGTH = "USER_003";
        public const string INVALID_BIRTH_DATE = "USER_004";
        public const string INVALID_AVATAR_URL = "USER_005";
        public const string INVALID_BIO_LENGTH = "USER_006";
        public const string USER_ARGUMENT_NULL = "USER_007";
        public const string USER_NOT_FOUND = "USER_008";
        public const string USER_UPDATE_FAILED = "USER_009";

        // System Errors
        public const string UNEXPECTED_ERROR = "SYS_001";
        public const string DATABASE_ERROR = "SYS_002";
        public const string EXTERNAL_SERVICE_ERROR = "SYS_003";
        public const string VALIDATION_ERROR = "SYS_004";
    }
}
