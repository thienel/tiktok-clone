using System;
using System.Linq;
using AutoFixture;
using AutoFixture.Xunit2;
using FluentAssertions;
using Moq;
using TikTokClone.Domain.Entities;
using TikTokClone.Domain.Event;
using TikTokClone.Domain.Exceptions;
using Xunit;

namespace TikTokClone.Tests.Domain.Entities
{
    public class UserTests
    {
        private readonly IFixture _fixture;
        private readonly string _validEmail;
        private readonly string _validUserName;
        private readonly DateOnly _validBirthDate;

        public UserTests()
        {
            _fixture = new Fixture();
            _validEmail = "test@example.com";
            _validUserName = "testuser123";
            _validBirthDate = DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-20));
        }

        #region Constructor Tests

        [Theory, AutoData]
        public void Constructor_WithValidInputs_ShouldCreateUserSuccessfully(
            string email,
            string userName)
        {
            // Arrange
            var validEmail = $"{email.Replace("@", "").Replace(".", "")}@example.com";
            var validUserName = userName.ToLower().Replace(" ", "").Substring(0, Math.Min(userName.Length, 10));
            var validBirthDate = _validBirthDate;

            // Act
            var user = new User(validEmail, validBirthDate, validUserName);

            // Assert
            user.Id.Should().NotBeEmpty();
            user.Email.Should().Be(validEmail);
            user.UserName.Should().Be(validUserName);
            user.Name.Should().Be(validUserName);
            user.BirthDate.Should().Be(validBirthDate);
            user.EmailConfirmed.Should().BeFalse();
            user.IsVerified.Should().BeFalse();
            user.Bio.Should().Be(User.BioDefaultValue);
            user.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
            user.LastUpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
            user.LastLoginAt.Should().BeNull();
            user.DomainEvents.Should().HaveCount(1);
            user.DomainEvents.First().Should().BeOfType<UserCreatedEvent>();
        }

        [Fact]
        public void Constructor_WithPredefinedValidInputs_ShouldCreateUserSuccessfully()
        {
            // Act
            var user = new User(_validEmail, _validBirthDate, _validUserName);

            // Assert
            user.Should().NotBeNull();
            user.Id.Should().NotBeEmpty();
            user.Email.Should().Be(_validEmail);
            user.UserName.Should().Be(_validUserName);
            user.Name.Should().Be(_validUserName);
            user.BirthDate.Should().Be(_validBirthDate);
            user.EmailConfirmed.Should().BeFalse();
            user.IsVerified.Should().BeFalse();
            user.Bio.Should().Be(User.BioDefaultValue);
            user.CreatedAt.Should().BeAfter(DateTime.MinValue);
            user.LastUpdatedAt.Should().BeAfter(DateTime.MinValue);
            user.LastLoginAt.Should().BeNull();
            user.DomainEvents.Should().ContainSingle()
                .Which.Should().BeOfType<UserCreatedEvent>();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Constructor_WithInvalidEmail_ShouldThrowException(string? email)
        {
            // Act & Assert
            var act = () => new User(email!, _validBirthDate, _validUserName);
            act.Should().Throw<UserArgumentNullException>();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Constructor_WithInvalidUserName_ShouldThrowException(string? userName)
        {
            // Act & Assert
            var act = () => new User(_validEmail, _validBirthDate, userName!);
            act.Should().Throw<UserArgumentNullException>();
        }

        [Theory]
        [InlineData("invalid-email")]
        [InlineData("@example.com")]
        [InlineData("test@")]
        [InlineData("test.example.com")]
        public void Constructor_WithInvalidEmailFormat_ShouldThrowException(string email)
        {
            // Act & Assert
            var act = () => new User(email, _validBirthDate, _validUserName);
            act.Should().Throw<InvalidEmailFormatException>();
        }

        [Theory]
        [InlineData("a")]
        [InlineData("user@name")]
        [InlineData("user-name")]
        [InlineData("user name")]
        public void Constructor_WithInvalidUsernameFormat_ShouldThrowException(string userName)
        {
            // Act & Assert
            var act = () => new User(_validEmail, _validBirthDate, userName);
            act.Should().Throw<InvalidUsernameFormatException>();
        }

        [Fact]
        public void Constructor_WithUnderageUser_ShouldThrowException()
        {
            // Arrange
            var underageBirthDate = DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-10));

            // Act & Assert
            var act = () => new User(_validEmail, underageBirthDate, _validUserName);
            act.Should().Throw<InvalidBirthDateException>();
        }

        #endregion

        #region ChangeUserName Tests

        [Theory, AutoData]
        public void ChangeUserName_WithValidNewUserName_ShouldReturnTrueAndUpdateUserName1(string newUserName)
        {
            // Arrange
            var validNewUserName = newUserName.ToLower().Replace(" ", "").Substring(0, Math.Min(newUserName.Length, 10));
            var user = new User(_validEmail, _validBirthDate, _validUserName);
            user.ClearDomainEvents();

            // Act
            var result = user.ChangeUserName(validNewUserName);

            // Assert
            result.Should().BeTrue();
            user.UserName.Should().Be(validNewUserName);
            user.DomainEvents.Should().ContainSingle()
                .Which.Should().BeOfType<UserUsernameChangedEvent>();
        }

        [Fact]
        public void ChangeUserName_WithValidNewUserName_ShouldReturnTrueAndUpdateUserName2()
        {
            // Arrange
            var user = new User(_validEmail, _validBirthDate, _validUserName);
            var newUserName = "newusername";
            user.ClearDomainEvents();

            // Act
            var result = user.ChangeUserName(newUserName);

            // Assert
            result.Should().BeTrue();
            user.UserName.Should().Be(newUserName);
            user.DomainEvents.Should().ContainSingle()
                .Which.Should().BeOfType<UserUsernameChangedEvent>();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void ChangeUserName_WithInvalidUserName_ShouldReturnFalse(string? userName)
        {
            // Arrange
            var user = new User(_validEmail, _validBirthDate, _validUserName);
            user.ClearDomainEvents();

            // Act
            var result = user.ChangeUserName(userName);

            // Assert
            result.Should().BeFalse();
            user.UserName.Should().Be(_validUserName);
            user.DomainEvents.Should().BeEmpty();
        }

        [Fact]
        public void ChangeUserName_WithSameUserName_ShouldReturnFalse()
        {
            // Arrange
            var user = new User(_validEmail, _validBirthDate, _validUserName);
            user.ClearDomainEvents();

            // Act
            var result = user.ChangeUserName(_validUserName);

            // Assert
            result.Should().BeFalse();
            user.DomainEvents.Should().BeEmpty();
        }

        [Theory]
        [InlineData("a")]
        [InlineData("user@name")]
        public void ChangeUserName_WithInvalidFormat_ShouldReturnFalse(string userName)
        {
            // Arrange
            var user = new User(_validEmail, _validBirthDate, _validUserName);
            user.ClearDomainEvents();

            // Act
            var result = user.ChangeUserName(userName);

            // Assert
            result.Should().BeFalse();
            user.UserName.Should().Be(_validUserName);
            user.DomainEvents.Should().BeEmpty();
        }

        #endregion

        #region ChangeName Tests

        [Theory, AutoData]
        public void ChangeName_WithValidName_ShouldReturnTrueAndUpdateName1(string newName)
        {
            // Arrange
            var user = new User(_validEmail, _validBirthDate, _validUserName);
            user.ClearDomainEvents();

            // Act
            var result = user.ChangeName(newName);

            // Assert
            result.Should().BeTrue();
            user.Name.Should().Be(newName);
            user.DomainEvents.Should().ContainSingle()
                .Which.Should().BeOfType<UserProfileUpdatedEvent>();
        }

        [Fact]
        public void ChangeName_WithValidName_ShouldReturnTrueAndUpdateName2()
        {
            // Arrange
            var user = new User(_validEmail, _validBirthDate, _validUserName);
            var newName = "New Name";
            user.ClearDomainEvents();

            // Act
            var result = user.ChangeName(newName);

            // Assert
            result.Should().BeTrue();
            user.Name.Should().Be(newName);
            user.DomainEvents.Should().ContainSingle()
                .Which.Should().BeOfType<UserProfileUpdatedEvent>();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void ChangeName_WithInvalidName_ShouldReturnFalse(string? name)
        {
            // Arrange
            var user = new User(_validEmail, _validBirthDate, _validUserName);
            user.ClearDomainEvents();

            // Act
            var result = user.ChangeName(name);

            // Assert
            result.Should().BeFalse();
            user.Name.Should().Be(_validUserName);
            user.DomainEvents.Should().BeEmpty();
        }

        [Fact]
        public void ChangeName_WithTooLongName_ShouldThrowException()
        {
            // Arrange
            var user = new User(_validEmail, _validBirthDate, _validUserName);
            var longName = _fixture.Create<string>().PadRight(User.MaxNameLength + 1, 'a');

            // Act & Assert
            var act = () => user.ChangeName(longName);
            act.Should().Throw<InvalidNameLengthException>();
        }

        [Fact]
        public void ChangeName_WithSameName_ShouldReturnFalse()
        {
            // Arrange
            var user = new User(_validEmail, _validBirthDate, _validUserName);
            user.ClearDomainEvents();

            // Act
            var result = user.ChangeName(_validUserName);

            // Assert
            result.Should().BeFalse();
            user.DomainEvents.Should().BeEmpty();
        }

        #endregion

        #region ChangeAvatar Tests

        [Theory]
        [InlineData("https://example.com/avatar.jpg")]
        [InlineData("https://cdn.example.com/images/user123.png")]
        public void ChangeAvatar_WithValidUrl_ShouldReturnTrueAndUpdateAvatar(string avatarUrl)
        {
            // Arrange
            var user = new User(_validEmail, _validBirthDate, _validUserName);
            user.ClearDomainEvents();

            // Act
            var result = user.ChangeAvatar(avatarUrl);

            // Assert
            result.Should().BeTrue();
            user.AvatarURL.Should().Be(avatarUrl);
            user.DomainEvents.Should().ContainSingle()
                .Which.Should().BeOfType<UserAvatarChangedEvent>();
        }

        [Fact]
        public void ChangeAvatar_WithNullUrl_ShouldReturnTrueAndSetToNull()
        {
            // Arrange
            var user = new User(_validEmail, _validBirthDate, _validUserName);
            user.ChangeAvatar("https://example.com/avatar.jpg");
            user.ClearDomainEvents();

            // Act
            var result = user.ChangeAvatar(null);

            // Assert
            result.Should().BeTrue();
            user.AvatarURL.Should().BeNull();
            user.DomainEvents.Should().ContainSingle()
                .Which.Should().BeOfType<UserAvatarChangedEvent>();
        }

        [Theory]
        [InlineData("invalid-url")]
        [InlineData("not-a-url")]
        public void ChangeAvatar_WithInvalidUrl_ShouldThrowException(string avatarUrl)
        {
            // Arrange
            var user = new User(_validEmail, _validBirthDate, _validUserName);

            // Act & Assert
            var act = () => user.ChangeAvatar(avatarUrl);
            act.Should().Throw<InvalidAvatarUrlException>();
        }

        [Fact]
        public void ChangeAvatar_WithSameUrl_ShouldReturnFalse()
        {
            // Arrange
            var user = new User(_validEmail, _validBirthDate, _validUserName);
            var avatarUrl = "https://example.com/avatar.jpg";
            user.ChangeAvatar(avatarUrl);
            user.ClearDomainEvents();

            // Act
            var result = user.ChangeAvatar(avatarUrl);

            // Assert
            result.Should().BeFalse();
            user.DomainEvents.Should().BeEmpty();
        }

        #endregion

        #region ChangeBio Tests

        [Theory, AutoData]
        public void ChangeBio_WithValidBio_ShouldReturnTrueAndUpdateBio1(string newBio)
        {
            // Arrange
            var validBio = newBio.Length > User.MaxBioLength
                ? newBio.Substring(0, User.MaxBioLength)
                : newBio;
            var user = new User(_validEmail, _validBirthDate, _validUserName);
            user.ClearDomainEvents();

            // Act
            var result = user.ChangeBio(validBio);

            // Assert
            result.Should().BeTrue();
            user.Bio.Should().Be(validBio);
            user.DomainEvents.Should().ContainSingle()
                .Which.Should().BeOfType<UserProfileUpdatedEvent>();
        }

        [Fact]
        public void ChangeBio_WithValidBio_ShouldReturnTrueAndUpdateBio2()
        {
            // Arrange
            var user = new User(_validEmail, _validBirthDate, _validUserName);
            var newBio = "This is my new bio";
            user.ClearDomainEvents();

            // Act
            var result = user.ChangeBio(newBio);

            // Assert
            result.Should().BeTrue();
            user.Bio.Should().Be(newBio);
            user.DomainEvents.Should().ContainSingle()
                .Which.Should().BeOfType<UserProfileUpdatedEvent>();
        }

        [Fact]
        public void ChangeBio_WithNullBio_ShouldReturnTrueAndSetToNull()
        {
            // Arrange
            var user = new User(_validEmail, _validBirthDate, _validUserName);
            user.ClearDomainEvents();

            // Act
            var result = user.ChangeBio(null);

            // Assert
            result.Should().BeTrue();
            user.Bio.Should().BeNull();
            user.DomainEvents.Should().ContainSingle()
                .Which.Should().BeOfType<UserProfileUpdatedEvent>();
        }

        [Fact]
        public void ChangeBio_WithTooLongBio_ShouldThrowException()
        {
            // Arrange
            var user = new User(_validEmail, _validBirthDate, _validUserName);
            var longBio = _fixture.Create<string>().PadRight(User.MaxBioLength + 1, 'a');

            // Act & Assert
            var act = () => user.ChangeBio(longBio);
            act.Should().Throw<InvalidBioLengthException>();
        }

        [Fact]
        public void ChangeBio_WithSameBio_ShouldReturnFalse()
        {
            // Arrange
            var user = new User(_validEmail, _validBirthDate, _validUserName);
            user.ClearDomainEvents();

            // Act
            var result = user.ChangeBio(User.BioDefaultValue);

            // Assert
            result.Should().BeFalse();
            user.DomainEvents.Should().BeEmpty();
        }

        #endregion

        #region RecordLogin Tests

        [Fact]
        public void RecordLogin_ShouldUpdateLastLoginAtAndAddEvent()
        {
            // Arrange
            var user = new User(_validEmail, _validBirthDate, _validUserName);
            var beforeLogin = DateTime.UtcNow;
            user.ClearDomainEvents();

            // Act
            user.RecordLogin();

            // Assert
            user.LastLoginAt.Should().NotBeNull()
                .And.BeOnOrAfter(beforeLogin)
                .And.BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
            user.DomainEvents.Should().ContainSingle()
                .Which.Should().BeOfType<UserLoginRecordedEvent>();
        }

        #endregion

        #region Verify/Unverify Tests

        [Fact]
        public void Verify_WhenNotVerified_ShouldSetVerifiedAndAddEvent()
        {
            // Arrange
            var user = new User(_validEmail, _validBirthDate, _validUserName);
            user.ClearDomainEvents();

            // Act
            user.Verify();

            // Assert
            user.IsVerified.Should().BeTrue();
            user.DomainEvents.Should().ContainSingle()
                .Which.Should().BeOfType<UserVerifiedEvent>();
        }

        [Fact]
        public void Verify_WhenAlreadyVerified_ShouldNotAddEvent()
        {
            // Arrange
            var user = new User(_validEmail, _validBirthDate, _validUserName);
            user.Verify();
            user.ClearDomainEvents();

            // Act
            user.Verify();

            // Assert
            user.IsVerified.Should().BeTrue();
            user.DomainEvents.Should().BeEmpty();
        }

        [Fact]
        public void UnVerify_WhenVerified_ShouldSetUnverifiedAndAddEvent()
        {
            // Arrange
            var user = new User(_validEmail, _validBirthDate, _validUserName);
            user.Verify();
            user.ClearDomainEvents();

            // Act
            user.UnVerify();

            // Assert
            user.IsVerified.Should().BeFalse();
            user.DomainEvents.Should().ContainSingle()
                .Which.Should().BeOfType<UserUnverifiedEvent>();
        }

        [Fact]
        public void UnVerify_WhenNotVerified_ShouldNotAddEvent()
        {
            // Arrange
            var user = new User(_validEmail, _validBirthDate, _validUserName);
            user.ClearDomainEvents();

            // Act
            user.UnVerify();

            // Assert
            user.IsVerified.Should().BeFalse();
            user.DomainEvents.Should().BeEmpty();
        }

        #endregion

        #region RequiresReAuthentication Tests

        [Fact]
        public void RequiresReAuthentication_WhenNeverLoggedIn_ShouldReturnTrue()
        {
            // Arrange
            var user = new User(_validEmail, _validBirthDate, _validUserName);

            // Act
            var result = user.RequiresReAuthentication();

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void RequiresReAuthentication_WhenRecentLogin_ShouldReturnFalse()
        {
            // Arrange
            var user = new User(_validEmail, _validBirthDate, _validUserName);
            user.RecordLogin();

            // Act
            var result = user.RequiresReAuthentication();

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void RequiresReAuthentication_WhenOldLogin_ShouldReturnTrue()
        {
            // Arrange
            var user = new User(_validEmail, _validBirthDate, _validUserName);
            user.RecordLogin();

            // Using reflection to set old login date for testing
            var lastLoginProperty = typeof(User).GetProperty("LastLoginAt");
            lastLoginProperty?.SetValue(user, DateTime.UtcNow.AddDays(-31));

            // Act
            var result = user.RequiresReAuthentication();

            // Assert
            result.Should().BeTrue();
        }

        #endregion

        #region ConfirmEmail Tests

        [Fact]
        public void ConfirmEmail_WhenNotConfirmed_ShouldReturnTrueAndSetConfirmed()
        {
            // Arrange
            var user = new User(_validEmail, _validBirthDate, _validUserName);
            user.ClearDomainEvents();

            // Act
            var result = user.ConfirmEmail();

            // Assert
            result.Should().BeTrue();
            user.EmailConfirmed.Should().BeTrue();
            user.DomainEvents.Should().ContainSingle()
                .Which.Should().BeOfType<UserEmailConfirmedEvent>();
        }

        [Fact]
        public void ConfirmEmail_WhenAlreadyConfirmed_ShouldReturnFalse()
        {
            // Arrange
            var user = new User(_validEmail, _validBirthDate, _validUserName);
            user.ConfirmEmail();
            user.ClearDomainEvents();

            // Act
            var result = user.ConfirmEmail();

            // Assert
            result.Should().BeFalse();
            user.DomainEvents.Should().BeEmpty();
        }

        #endregion

        #region Static Method Tests

        [Theory]
        [InlineData("test@example.com", true)]
        [InlineData("user.name@domain.co.uk", true)]
        [InlineData("invalid-email", false)]
        [InlineData("@example.com", false)]
        [InlineData("test@", false)]
        [InlineData("", false)]
        [InlineData(null, false)]
        public void IsValidEmail_ShouldReturnExpectedResult(string? email, bool expected)
        {
            // Act
            var result = User.IsValidEmail(email!);

            // Assert
            result.Should().Be(expected);
        }

        [Fact]
        public void IsValidBirthDate_WithValidAge_ShouldReturnTrue()
        {
            // Arrange
            var validBirthDate = DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-20));

            // Act
            var result = User.IsValidBirthDate(validBirthDate);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void IsValidBirthDate_WithUnderageUser_ShouldReturnFalse()
        {
            // Arrange
            var underageBirthDate = DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-10));

            // Act
            var result = User.IsValidBirthDate(underageBirthDate);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void IsValidBirthDate_WithExactMinimumAge_ShouldReturnTrue()
        {
            // Arrange
            var exactMinimumAgeBirthDate = DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-User.MinimumRequiredAge));

            // Act
            var result = User.IsValidBirthDate(exactMinimumAgeBirthDate);

            // Assert
            result.Should().BeTrue();
        }

        [Theory, AutoData]
        public void IsValidEmail_WithAutoGeneratedData_ShouldValidateCorrectly(string localPart, string domain)
        {
            // Arrange
            var validEmail = $"{localPart.Replace("@", "").Replace(".", "")}@{domain.Replace("@", "").Replace(".", "")}.com";
            var invalidEmail = $"{localPart}@";

            // Act & Assert
            User.IsValidEmail(validEmail).Should().BeTrue();
            User.IsValidEmail(invalidEmail).Should().BeFalse();
        }

        #endregion

        #region Domain Events Tests

        [Fact]
        public void ClearDomainEvents_ShouldRemoveAllEvents()
        {
            // Arrange
            var user = new User(_validEmail, _validBirthDate, _validUserName);
            user.ChangeName("New Name");
            user.ChangeAvatar("https://example.com/avatar.jpg");

            user.DomainEvents.Should().HaveCountGreaterThan(1);

            // Act
            user.ClearDomainEvents();

            // Assert
            user.DomainEvents.Should().BeEmpty();
        }

        [Fact]
        public void DomainEvents_AfterMultipleOperations_ShouldContainCorrectEvents()
        {
            // Arrange
            var user = new User(_validEmail, _validBirthDate, _validUserName);
            user.ClearDomainEvents();

            // Act
            user.ChangeName("New Name");
            user.ChangeAvatar("https://example.com/avatar.jpg");
            user.Verify();
            user.RecordLogin();

            // Assert
            user.DomainEvents.Should().HaveCount(4);
            user.DomainEvents.Should().ContainItemsAssignableTo<UserProfileUpdatedEvent>();
            user.DomainEvents.Should().ContainItemsAssignableTo<UserAvatarChangedEvent>();
            user.DomainEvents.Should().ContainItemsAssignableTo<UserVerifiedEvent>();
            user.DomainEvents.Should().ContainItemsAssignableTo<UserLoginRecordedEvent>();
        }

        #endregion

        #region Integration Tests with AutoFixture

        [Theory, AutoData]
        public void User_WithAutoFixtureData_ShouldHandleComplexScenarios(
            string email,
            string userName,
            string displayName,
            string bio)
        {
            // Arrange
            var validEmail = $"{email.Replace("@", "").Replace(".", "").ToLower()}@example.com";
            var validUserName = userName.ToLower().Replace(" ", "").Replace("@", "").Replace("-", "")
                .Substring(0, Math.Min(userName.Length, 15));
            var validDisplayName = displayName.Length > User.MaxNameLength
                ? displayName.Substring(0, User.MaxNameLength)
                : displayName;
            var validBio = bio.Length > User.MaxBioLength
                ? bio.Substring(0, User.MaxBioLength)
                : bio;

            // Act
            var user = new User(validEmail, _validBirthDate, validUserName);
            user.ClearDomainEvents();

            var nameChanged = user.ChangeName(validDisplayName);
            var bioChanged = user.ChangeBio(validBio);
            user.Verify();
            user.RecordLogin();

            // Assert
            user.Email.Should().Be(validEmail);
            user.UserName.Should().Be(validUserName);
            nameChanged.Should().BeTrue();
            user.Name.Should().Be(validDisplayName);
            bioChanged.Should().BeTrue();
            user.Bio.Should().Be(validBio);
            user.IsVerified.Should().BeTrue();
            user.LastLoginAt.Should().NotBeNull();
            user.DomainEvents.Should().HaveCount(4); // ChangeName, ChangeBio, Verify, RecordLogin
        }

        #endregion
    }
}
