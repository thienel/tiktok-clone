using System;
using System.Text.RegularExpressions;
using Xunit;

public class UserTests
{
    [Fact]
    public void Constructor_ShouldInitializeFieldsCorrectly()
    {
        var user = new User("John", "Doe", "johndoe");

        Assert.Equal("John", user.FirstName);
        Assert.Equal("Doe", user.LastName);
        Assert.Equal("johndoe", user.UserName);
        Assert.False(user.IsVerify);
        Assert.Null(user.AvatarURL);
        Assert.Null(user.Bio);
        Assert.Equal(user.CreatedAt, user.LastUpdatedAt, TimeSpan.FromMilliseconds(10));
    }

    [Fact]
    public void Constructor_ShouldGenerateUserName_WhenNullIsPassed()
    {
        var user = new User("John", "Doe", null);

        Assert.False(string.IsNullOrWhiteSpace(user.UserName));
        Assert.Matches(@"^[0-9a-f\-]{36}$", user.UserName); // GUID format
    }

    [Theory]
    [InlineData("validUser123", true)]
    [InlineData("invalid user", false)]
    [InlineData("", false)]
    [InlineData("user@name", false)]
    public void ChangeUserName_ShouldValidateUserName(string newUserName, bool expectedResult)
    {
        var user = new User("John", "Doe", "olduser");
        var oldTime = user.LastUpdatedAt;

        var result = user.ChangeUserName(newUserName);

        Assert.Equal(expectedResult, result);
        if (expectedResult)
        {
            Assert.Equal(newUserName, user.UserName);
            Assert.True(user.LastUpdatedAt > oldTime);
        }
        else
        {
            Assert.Equal("olduser", user.UserName);
        }
    }

    [Fact]
    public void ChangeName_ShouldUpdate_WhenDifferent()
    {
        var user = new User("John", "Doe", "johndoe");
        var oldTime = user.LastUpdatedAt;

        var result = user.ChangeName("Jane", "Smith");

        Assert.True(result);
        Assert.Equal("Jane", user.FirstName);
        Assert.Equal("Smith", user.LastName);
        Assert.True(user.LastUpdatedAt > oldTime);
    }

    [Fact]
    public void ChangeName_ShouldNotUpdate_WhenSame()
    {
        var user = new User("John", "Doe", "johndoe");
        var oldTime = user.LastUpdatedAt;

        var result = user.ChangeName("John", "Doe");

        Assert.False(result);
        Assert.Equal(oldTime, user.LastUpdatedAt);
    }

    [Fact]
    public void ChangeAvatar_ShouldUpdateAvatarAndTime()
    {
        var user = new User("John", "Doe", "johndoe");
        var oldTime = user.LastUpdatedAt;

        user.ChangeAvatar("http://example.com/avatar.jpg");

        Assert.Equal("http://example.com/avatar.jpg", user.AvatarURL);
        Assert.True(user.LastUpdatedAt > oldTime);
    }

    [Fact]
    public void ChangeBio_ShouldUpdateBioAndTime()
    {
        var user = new User("John", "Doe", "johndoe");
        var oldTime = user.LastUpdatedAt;

        user.ChangeBio("New bio here.");

        Assert.Equal("New bio here.", user.Bio);
        Assert.True(user.LastUpdatedAt > oldTime);
    }

    [Fact]
    public void Verify_ShouldSetIsVerifyToTrue()
    {
        var user = new User("John", "Doe", "johndoe");

        user.Verify();

        Assert.True(user.IsVerify);
    }

    [Fact]
    public void UnVerify_ShouldSetIsVerifyToFalse()
    {
        var user = new User("John", "Doe", "johndoe");
        user.Verify();

        user.UnVerify();

        Assert.False(user.IsVerify);
    }
}
