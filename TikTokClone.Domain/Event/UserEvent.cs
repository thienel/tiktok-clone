
namespace TikTokClone.Domain.Event
{

    public class UserCreatedEvent : DomainEvent
    {
        public string UserId { get; }
        public string Email { get; }
        public string UserName { get; }
        public string Name { get; }
        public DateOnly BirthDate { get; }

        public UserCreatedEvent(string userId, string email, string userName, string name, DateOnly birthDate)
        {
            UserId = userId;
            Email = email;
            UserName = userName;
            Name = name;
            BirthDate = birthDate;
        }
    }

    public class UserEmailConfirmedEvent : DomainEvent
    {
        public string UserId { get; }
        public string Email { get; }

        public UserEmailConfirmedEvent(string userId, string email)
        {
            UserId = userId;
            Email = email;
        }
    }

    public class UserProfileUpdatedEvent : DomainEvent
    {
        public string UserId { get; }
        public string? PropertyName { get; }
        public object? OldValue { get; }
        public object? NewValue { get; }

        public UserProfileUpdatedEvent(string userId, string? propertyName, object? oldValue, object? newValue)
        {
            UserId = userId;
            PropertyName = propertyName;
            OldValue = oldValue;
            NewValue = newValue;
        }
    }

    public class UserVerifiedEvent : DomainEvent
    {
        public string UserId { get; }
        public string UserName { get; }

        public UserVerifiedEvent(string userId, string userName)
        {
            UserId = userId;
            UserName = userName;
        }
    }

    public class UserUnverifiedEvent : DomainEvent
    {
        public string UserId { get; }
        public string UserName { get; }

        public UserUnverifiedEvent(string userId, string userName)
        {
            UserId = userId;
            UserName = userName;
        }
    }

    public class UserLoginRecordedEvent : DomainEvent
    {
        public string UserId { get; }
        public DateTime LoginTime { get; }

        public UserLoginRecordedEvent(string userId, DateTime loginTime)
        {
            UserId = userId;
            LoginTime = loginTime;
        }
    }

    public class UserUsernameChangedEvent : DomainEvent
    {
        public string UserId { get; }
        public string OldUsername { get; }
        public string NewUsername { get; }

        public UserUsernameChangedEvent(string userId, string oldUsername, string newUsername)
        {
            UserId = userId;
            OldUsername = oldUsername;
            NewUsername = newUsername;
        }
    }

    public class UserAvatarChangedEvent : DomainEvent
    {
        public string UserId { get; }
        public string? OldAvatarUrl { get; }
        public string? NewAvatarUrl { get; }

        public UserAvatarChangedEvent(string userId, string? oldAvatarUrl, string? newAvatarUrl)
        {
            UserId = userId;
            OldAvatarUrl = oldAvatarUrl;
            NewAvatarUrl = newAvatarUrl;
        }
    }
}
