using AuthService.Constants;
using AuthService.Domain.Exceptions;

namespace AuthService.Mappers
{
    public static class ErrorCodeMapper
    {
        private static readonly Dictionary<string, string> DomainToApplicationErrorMap = new()
        {
            { InvalidEmailFormatException.Code, ErrorCodes.INVALID_EMAIL_FORMAT },
            { InvalidUsernameFormatException.Code, ErrorCodes.INVALID_USERNAME_FORMAT },
            { InvalidNameLengthException.Code, ErrorCodes.INVALID_NAME_LENGTH },
            { InvalidBirthDateException.Code, ErrorCodes.INVALID_BIRTH_DATE },
            { InvalidAvatarUrlException.Code, ErrorCodes.INVALID_AVATAR_URL },
            { InvalidBioLengthException.Code, ErrorCodes.INVALID_BIO_LENGTH },
            { UserArgumentNullException.Code, ErrorCodes.USER_ARGUMENT_NULL }
        };

        public static string MapDomainErrorToApplication(string domainErrorCode)
        {
            return DomainToApplicationErrorMap.TryGetValue(domainErrorCode, out var applicationCode)
                ? applicationCode
                : ErrorCodes.UNEXPECTED_ERROR;
        }

        public static string GetApplicationErrorFromDomainException(DomainException domainException)
        {
            return MapDomainErrorToApplication(domainException.ErrorCode);
        }
    }
}
