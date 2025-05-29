using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Identity;
using TikTokClone.Domain.Event;

namespace TikTokClone.Domain.Entities
{
    public class User : IdentityUser
    {
        public string FirstName { get; private set; }
        public string LastName { get; private set; }
        public string? AvatarURL { get; private set; }
        public bool IsVerified { get; private set; }
        public string? Bio { get; private set; }
        public DateTime CreatedAt { get; init; }
        public DateTime LastUpdatedAt { get; private set; }
        public DateTime? LastLoginAt { get; private set; }

        private readonly List<IDomainEvent> _domainEvents = new();
        public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

        private static readonly Regex _userNameRegex = new(@"^[a-z0-9._]{3,30}$", RegexOptions.Compiled);
        private static readonly Regex _emailRegex = new(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.Compiled);

        public const int MaxBioLength = 500;
        public const int MaxFirstNameLength = 50;
        public const int MaxLastNameLength = 50;

        public User(string email, string firstName, string lastName, string? userName = null)
        {
            ValidateConstructorInputs(email, firstName, lastName);

            Id = Guid.NewGuid().ToString();
            Email = email.Trim().ToLower();
            UserName = userName ?? GenerateDefaultUserName(firstName, lastName);
            FirstName = firstName.Trim();
            LastName = lastName.Trim();
            EmailConfirmed = false;
            IsVerified = false;
            CreatedAt = DateTime.UtcNow;
            LastUpdatedAt = DateTime.UtcNow;

            _domainEvents.Add(new UserCreatedEvent(this));
        }

        private User() { }

        public string FullName => $"{FirstName} {LastName}";

        public bool ChangeUserName(string? userName)
        {
            if (string.IsNullOrWhiteSpace(userName))
                return false;

            userName = userName.ToLower().Trim();

            if (!_userNameRegex.IsMatch(userName))
                throw new DomainException("Username must be 3-30 characters and contain only lowercase letters, numbers, dots, and underscores.");

            if (userName == UserName)
                return false;

            var oldUserName = UserName;
            UserName = userName;
            ChangeUpdateTime();

            _domainEvents.Add(new UserNameChangedEvent(this, oldUserName, userName));
            return true;
        }

        public bool ChangeName(string? firstName, string? lastName)
        {
            bool isUpdated = false;

            if (!string.IsNullOrWhiteSpace(firstName))
            {
                var trimmedFirstName = firstName.Trim();
                if (trimmedFirstName.Length > MaxFirstNameLength)
                    throw new DomainException($"First name cannot exceed {MaxFirstNameLength} characters.");

                if (trimmedFirstName != FirstName)
                {
                    FirstName = trimmedFirstName;
                    isUpdated = true;
                }
            }

            if (!string.IsNullOrWhiteSpace(lastName))
            {
                var trimmedLastName = lastName.Trim();
                if (trimmedLastName.Length > MaxLastNameLength)
                    throw new DomainException($"Last name cannot exceed {MaxLastNameLength} characters.");

                if (trimmedLastName != LastName)
                {
                    LastName = trimmedLastName;
                    isUpdated = true;
                }
            }

            if (isUpdated)
            {
                ChangeUpdateTime();
                _domainEvents.Add(new UserNameChangedEvent(this));
            }

            return isUpdated;
        }

        public bool ChangeAvatar(string? avatarUrl)
        {
            var newAvatarUrl = string.IsNullOrWhiteSpace(avatarUrl) ? null : avatarUrl.Trim();

            if (newAvatarUrl != null && !Uri.TryCreate(newAvatarUrl, UriKind.Absolute, out _))
                throw new DomainException("Invalid avatar URL format.");

            if (newAvatarUrl != AvatarURL)
            {
                AvatarURL = newAvatarUrl;
                ChangeUpdateTime();
                _domainEvents.Add(new UserAvatarChangedEvent(this, newAvatarUrl));
                return true;
            }
            return false;
        }

        public bool ChangeBio(string? bio)
        {
            var newBio = string.IsNullOrWhiteSpace(bio) ? null : bio.Trim();

            if (newBio != null && newBio.Length > MaxBioLength)
                throw new DomainException($"Bio cannot exceed {MaxBioLength} characters.");

            if (newBio != Bio)
            {
                Bio = newBio;
                ChangeUpdateTime();
                _domainEvents.Add(new UserBioChangedEvent(this, newBio));
                return true;
            }
            return false;
        }

        public void ConfirmEmail()
        {
            if (!EmailConfirmed)
            {
                EmailConfirmed = true;
                ChangeUpdateTime();
                _domainEvents.Add(new UserEmailConfirmedEvent(this));
            }
        }

        public void RecordLogin()
        {
            LastLoginAt = DateTime.UtcNow;
            ChangeUpdateTime();
        }

        public bool CanChangePassword()
        {
            return EmailConfirmed;
        }

        public void Verify()
        {
            if (!IsVerified)
            {
                IsVerified = true;
                ChangeUpdateTime();
                _domainEvents.Add(new UserVerifiedEvent(this));
            }
        }

        public void UnVerify()
        {
            if (IsVerified)
            {
                IsVerified = false;
                ChangeUpdateTime();
                _domainEvents.Add(new UserUnverifiedEvent(this));
            }
        }

        public bool RequiresReAuthentication()
        {
            return LastLoginAt == null ||
                   DateTime.UtcNow.Subtract(LastLoginAt.Value).TotalDays > 30;
        }

        public void ClearDomainEvents()
        {
            _domainEvents.Clear();
        }

        private void ChangeUpdateTime()
        {
            LastUpdatedAt = DateTime.UtcNow;
        }

        private void ValidateConstructorInputs(string email, string firstName, string lastName)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email cannot be null or empty", nameof(email));
            if (string.IsNullOrWhiteSpace(firstName))
                throw new ArgumentException("FirstName cannot be null or empty", nameof(firstName));
            if (string.IsNullOrWhiteSpace(lastName))
                throw new ArgumentException("LastName cannot be null or empty", nameof(lastName));

            if (!IsValidEmail(email))
                throw new DomainException("Invalid email format.");
            if (firstName.Trim().Length > MaxFirstNameLength)
                throw new DomainException($"First name cannot exceed {MaxFirstNameLength} characters.");
            if (lastName.Trim().Length > MaxLastNameLength)
                throw new DomainException($"Last name cannot exceed {MaxLastNameLength} characters.");
        }

        private string GenerateDefaultUserName(string firstName, string lastName)
        {
            var baseUserName = $"{firstName.ToLower()}.{lastName.ToLower()}";
            return baseUserName;
        }

        public static bool IsValidEmail(string email)
        {
            return !string.IsNullOrWhiteSpace(email) &&
                   email.Length <= 256 &&
                   _emailRegex.IsMatch(email);
        }
    }
}
