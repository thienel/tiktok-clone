namespace AuthService.Domain.Exceptions
{
    public class InvalidEmailFormatException : DomainException
    {
        public const string Code = "USER_001";

        public InvalidEmailFormatException() : base("Invalid email format.", Code)
        {
        }
    }

    public class InvalidUsernameFormatException : DomainException
    {
        public const string Code = "USER_002";

        public InvalidUsernameFormatException() : base("Username must be 2-24 characters and contain only lowercase letters, numbers, dots, and underscores.", Code)
        {
        }
    }

    public class InvalidNameLengthException : DomainException
    {
        public const string Code = "USER_003";

        public InvalidNameLengthException(int maxLength) : base($"Name cannot exceed {maxLength} characters.", Code)
        {
        }
    }

    public class InvalidBirthDateException : DomainException
    {
        public const string Code = "USER_004";

        public InvalidBirthDateException(int minimumAge) : base($"Age must be at least {minimumAge} years old.", Code)
        {
        }
    }

    public class InvalidAvatarUrlException : DomainException
    {
        public const string Code = "USER_005";

        public InvalidAvatarUrlException() : base("Invalid avatar URL format.", Code)
        {
        }
    }

    public class InvalidBioLengthException : DomainException
    {
        public const string Code = "USER_006";

        public InvalidBioLengthException(int maxLength) : base($"Bio cannot exceed {maxLength} characters.", Code)
        {
        }
    }

    public class UserArgumentNullException : DomainException
    {
        public const string Code = "USER_007";

        public UserArgumentNullException(string parameterName) : base($"{parameterName} cannot be null or empty.", Code)
        {
        }
    }
}
