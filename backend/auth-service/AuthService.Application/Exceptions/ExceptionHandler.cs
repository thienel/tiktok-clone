using AuthService.Constants;
using AuthService.Mappers;
using AuthService.Domain.Exceptions;

namespace AuthService.Exceptions
{
    public static class ExceptionHandler
    {
        public static (string ErrorCode, string Message) HandleDomainException(DomainException domainException)
        {
            var errorCode = ErrorCodeMapper.GetApplicationErrorFromDomainException(domainException);
            return (errorCode, domainException.Message);
        }

        public static (string ErrorCode, string Message) HandleGenericException(Exception exception)
        {
            return (ErrorCodes.UNEXPECTED_ERROR, "An unexpected error occurred");
        }
    }
}
