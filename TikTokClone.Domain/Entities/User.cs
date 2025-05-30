using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Identity;
using TikTokClone.Domain.Event;

namespace TikTokClone.Domain.Entities
{
    public class User : IdentityUser
    {
        public string Name { get; private set; }
        public string? AvatarURL { get; private set; }
        public bool IsVerified { get; private set; }
        public string? Bio { get; private set; }
        public DateOnly BirthDate { get; init; }
        public DateTime CreatedAt { get; init; }
        public DateTime LastUpdatedAt { get; private set; }
        public DateTime? LastLoginAt { get; private set; }

        private readonly List<IDomainEvent> _domainEvents = new();
        public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

        private static readonly Regex _userNameRegex = new(@"^[a-z0-9._]{2,24}$", RegexOptions.Compiled);
        private static readonly Regex _emailRegex = new(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.Compiled);

        public const int MaxBioLength = 80;
        public const int MaxNameLength = 50;
        public const string BioDefaultValue = "No bio yet.";
        public const int MinimumRequiredAge = 12;

        public User(string email, string name, DateOnly birthDate, string userName)
        {
            ValidateConstructorInputs(email, name, birthDate, userName);

            Id = Guid.NewGuid().ToString();
            Email = email.Trim().ToLower();
            UserName = userName.Trim().ToLower();
            Name = name.Trim().ToLower();
            BirthDate = birthDate;
            EmailConfirmed = false;
            IsVerified = false;
            Bio = BioDefaultValue;
            CreatedAt = DateTime.UtcNow;
            LastUpdatedAt = DateTime.UtcNow;
        }

        private User() { }

        public bool ChangeUserName(string? userName)
        {
            if (string.IsNullOrWhiteSpace(userName))
                return false;

            userName = userName.ToLower().Trim();

            if (userName == UserName || !_userNameRegex.IsMatch(userName))
                return false;

            UserName = userName;
            ChangeUpdateTime();

            return true;
        }

        public bool ChangeName(string? name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return false;

            name = name.Trim();

            if (name.Length > MaxNameLength || name == Name)
                return false;

            Name = name;
            ChangeUpdateTime();
            return true;
        }

        public bool ChangeAvatar(string? avatarUrl)
        {
            avatarUrl = string.IsNullOrWhiteSpace(avatarUrl) ? null : avatarUrl.Trim();

            if (avatarUrl != null && !Uri.TryCreate(avatarUrl, UriKind.Absolute, out _))
                throw new DomainException("Invalid avatar URL format.");

            if (avatarUrl != AvatarURL)
            {
                AvatarURL = avatarUrl;
                ChangeUpdateTime();
                return true;
            }

            return false;
        }

        public bool ChangeBio(string? bio)
        {
            bio = string.IsNullOrWhiteSpace(bio) ? null : bio.Trim();

            if ((bio != null && bio.Length > MaxBioLength) || bio == Bio)
                return false;

            Bio = bio;
            ChangeUpdateTime();

            return true;
        }

        public void ConfirmEmail()
        {
            if (!EmailConfirmed)
            {
                EmailConfirmed = true;
                ChangeUpdateTime();
            }
        }

        public void RecordLogin()
        {
            LastLoginAt = DateTime.UtcNow;
        }

        public void Verify()
        {
            if (!IsVerified)
            {
                IsVerified = true;
                ChangeUpdateTime();
            }
        }

        public void UnVerify()
        {
            if (IsVerified)
            {
                IsVerified = false;
                ChangeUpdateTime();
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

        private void ValidateConstructorInputs(string email, string name, DateOnly birthDate, string userName)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email cannot be null or empty", nameof(email));
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Name cannot be null or empty", nameof(name));
            if (string.IsNullOrWhiteSpace(userName))
                throw new ArgumentException("userName cannot be null or empty", nameof(userName));

            if (!IsValidEmail(email))
                throw new DomainException("Invalid email format.");
            if (name.Trim().Length > MaxNameLength)
                throw new DomainException($"First name cannot exceed {MaxNameLength} characters.");
            if (!_userNameRegex.IsMatch(userName.Trim().ToLower()))
                throw new DomainException("Username must be 2-24 characters and contain only lowercase letters, numbers, dots, and underscores.");
            if (!IsValidBirthDate(birthDate))
                throw new DomainException("Birthdate indicates an age below the required minimum.");
        }

        public static bool IsValidEmail(string email)
        {
            return !string.IsNullOrWhiteSpace(email) &&
                   email.Length <= 256 &&
                   _emailRegex.IsMatch(email);
        }

        public static bool IsValidBirthDate(DateOnly birthDate)
        {
            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            int age = today.Year - birthDate.Year;

            if (birthDate > today.AddYears(-age))
                age--;

            return age >= MinimumRequiredAge;
        }
    }
}
