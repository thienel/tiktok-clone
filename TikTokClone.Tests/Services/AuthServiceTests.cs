using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Moq;
using TikTokClone.Application.Constants;
using TikTokClone.Application.DTOs;
using TikTokClone.Application.Interfaces.Repositories;
using TikTokClone.Application.Interfaces.Services;
using TikTokClone.Application.Interfaces.Settings;
using TikTokClone.Application.Services;
using TikTokClone.Domain.Entities;

namespace TikTokClone.Tests.Services
{
    public class AuthServiceTests
    {
        private readonly Mock<UserManager<User>> _userManagerMock;
        private readonly Mock<SignInManager<User>> _signInManagerMock;
        private readonly Mock<ITokenService> _tokenServiceMock;
        private readonly Mock<IRefreshTokenRepository> _refreshTokenRepoMock;
        private readonly Mock<IJwtSettings> _jwtSettingsMock;
        private readonly Mock<IEmailService> _emailServiceMock;
        private readonly Mock<IEmailCodeRepository> _emailCodeRepoMock;
        private readonly AuthService _authService;

        public AuthServiceTests()
        {
            _userManagerMock = MockUserManager();
            _signInManagerMock = MockSignInManager();
            _tokenServiceMock = new Mock<ITokenService>();
            _refreshTokenRepoMock = new Mock<IRefreshTokenRepository>();
            _jwtSettingsMock = new Mock<IJwtSettings>();
            _emailServiceMock = new Mock<IEmailService>();
            _emailCodeRepoMock = new Mock<IEmailCodeRepository>();

            _authService = new AuthService(
                _userManagerMock.Object,
                _signInManagerMock.Object,
                _tokenServiceMock.Object,
                _refreshTokenRepoMock.Object,
                _jwtSettingsMock.Object,
                _emailServiceMock.Object,
                _emailCodeRepoMock.Object
            );
        }

        [Fact]
        public async Task LoginAsync_WithValidCredentials_ReturnsSuccessResponse()
        {
            // Arrange
            var request = new LoginRequestDto
            {
                UsernameOrEmail = "test@example.com",
                Password = "ValidPassword123!"
            };

            var user = CreateTestUser();
            _userManagerMock.Setup(x => x.FindByEmailAsync(request.UsernameOrEmail))
                .ReturnsAsync(user);

            _signInManagerMock.Setup(x => x.CheckPasswordSignInAsync(user, request.Password, true))
                .ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Success);

            _userManagerMock.Setup(x => x.UpdateAsync(user))
                .ReturnsAsync(IdentityResult.Success);

            _tokenServiceMock.Setup(x => x.GenerateToken(user))
                .Returns("jwt-token");

            _tokenServiceMock.Setup(x => x.GenerateRefreshToken())
                .Returns("refresh-token");

            _jwtSettingsMock.Setup(x => x.RefreshTokenExpirationInDays).Returns(7);
            _jwtSettingsMock.Setup(x => x.ExpirationInMinutes).Returns(15);

            // Act
            var result = await _authService.LoginAsync(request);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal("Logged in successfully", result.Message);
            Assert.NotNull(result.Token);
            Assert.NotNull(result.RefreshToken);
        }

        [Fact]
        public async Task LoginAsync_WithInvalidCredentials_ReturnsFailureResponse()
        {
            // Arrange
            var request = new LoginRequestDto
            {
                UsernameOrEmail = "test@example.com",
                Password = "WrongPassword"
            };

            _userManagerMock.Setup(x => x.FindByEmailAsync(request.UsernameOrEmail))
                .ReturnsAsync((User)null!);

            // Act
            var result = await _authService.LoginAsync(request);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(ErrorCodes.INVALID_CREDENTIALS, result.ErrorCode);
        }

        private static User CreateTestUser()
        {
            return new User("test@example.com", DateOnly.FromDateTime(DateTime.Now.AddYears(-20)), "testuser");
        }

        private static Mock<UserManager<User>> MockUserManager()
        {
            var store = new Mock<IUserStore<User>>();
            return new Mock<UserManager<User>>(store.Object, null!, null!, null!, null!, null!, null!, null!, null!);
        }

        private static Mock<SignInManager<User>> MockSignInManager()
        {
            var userManager = MockUserManager();
            var contextAccessor = new Mock<Microsoft.AspNetCore.Http.IHttpContextAccessor>();
            var claimsFactory = new Mock<IUserClaimsPrincipalFactory<User>>();
            var options = new Mock<Microsoft.Extensions.Options.IOptions<IdentityOptions>>();
            var logger = new Mock<ILogger<SignInManager<User>>>();

            return new Mock<SignInManager<User>>(
                userManager.Object,
                contextAccessor.Object,
                claimsFactory.Object,
                options.Object,
                logger.Object,
                null!,
                null!
            );
        }
    }
}
