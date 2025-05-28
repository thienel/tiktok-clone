using System.Text.RegularExpressions;

public class User
{
    public string UserName { get; private set; }
    public string FirstName { get; private set; }
    public string LastName { get; private set; }
    public string? AvatarURL { get; private set; }
    public bool IsVerify { get; private set; }
    public string? Bio { get; private set; }
    public DateTime CreatedAt { get; init; }
    public DateTime LastUpdatedAt { get; private set; }

    private static readonly Regex _userNameRegex = new(@"^[A-Za-z0-9._]+$", RegexOptions.Compiled);


    public User(string firstName, string lastName, string? userName)
    {
        FirstName = firstName;
        LastName = lastName;
        UserName = userName ?? Guid.NewGuid().ToString();
        IsVerify = false;
        LastUpdatedAt = CreatedAt = DateTime.UtcNow;
    }

    public bool ChangeUserName(string userName)
    {
        if (!string.IsNullOrEmpty(userName) && _userNameRegex.IsMatch(userName))
        {
            UserName = userName;
            ChangeUpdateTime();
            return true;
        }

        return false;
    }

    public bool ChangeName(string firstName, string lastName)
    {
        if (firstName != FirstName || lastName != LastName)
        {
            FirstName = firstName;
            LastName = lastName;
            ChangeUpdateTime();
            return true;
        }

        return false;
    }

    public void ChangeAvatar(string avatarUrl)
    {
        AvatarURL = avatarUrl;
        ChangeUpdateTime();
    }

    public void ChangeBio(string bio)
    {
        Bio = bio;
        ChangeUpdateTime();
    }

    public void Verify()
    {
        IsVerify = true;
    }

    public void UnVerify()
    {
        IsVerify = false;
    }

    private void ChangeUpdateTime()
    {
        LastUpdatedAt = DateTime.UtcNow;
    }
}
