using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Identity;
using TikTokClone.Domain.Event;
using TikTokClone.Domain.Exceptions;

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
        public virtual ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();

        private readonly List<IDomainEvent> _domainEvents = new();
        public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

        private static readonly Regex _userNameRegex = new(@"^[a-z0-9._]{2,24}$", RegexOptions.Compiled);
        private static readonly Regex _emailRegex = new(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.Compiled);

        public const int MaxBioLength = 80;
        public const int MaxNameLength = 50;
        public const string BioDefaultValue = "No bio yet.";
        public const int MinimumRequiredAge = 12;

        public User(string email, DateOnly birthDate, string userName)
        {
            ValidateConstructorInputs(email, birthDate, userName);

            Id = Guid.NewGuid().ToString();
            Email = email.Trim().ToLower();
            UserName = userName.Trim().ToLower();
            Name = userName;
            BirthDate = birthDate;
            EmailConfirmed = false;
            IsVerified = false;
            Bio = BioDefaultValue;
            CreatedAt = DateTime.UtcNow;
            LastUpdatedAt = DateTime.UtcNow;

            _domainEvents.Add(new UserCreatedEvent(Id, Email, UserName, Name, BirthDate));
        }

        private User() { }

        public bool ChangeUserName(string? userName)
        {
            if (string.IsNullOrWhiteSpace(userName))
                return false;

            userName = userName.ToLower().Trim();

            if (userName == UserName || !_userNameRegex.IsMatch(userName))
                return false;

            var oldUsername = UserName;
            UserName = userName;
            ChangeUpdateTime();

            _domainEvents.Add(new UserUsernameChangedEvent(Id, oldUsername!, UserName));

            return true;
        }

        public bool ChangeName(string? name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return false;

            name = name.Trim();

            if (name.Length > MaxNameLength)
                throw new InvalidNameLengthException(MaxNameLength);

            if (name == Name)
                return false;

            var oldName = Name;
            Name = name;
            ChangeUpdateTime();

            _domainEvents.Add(new UserProfileUpdatedEvent(Id, nameof(Name), oldName, Name));

            return true;
        }

        public bool ChangeAvatar(string? avatarUrl)
        {
            avatarUrl = string.IsNullOrWhiteSpace(avatarUrl) ? null : avatarUrl.Trim();

            if (avatarUrl != null && !Uri.TryCreate(avatarUrl, UriKind.Absolute, out _))
                throw new InvalidAvatarUrlException();

            if (avatarUrl != AvatarURL)
            {
                var oldAvatarUrl = AvatarURL;
                AvatarURL = avatarUrl;
                ChangeUpdateTime();

                _domainEvents.Add(new UserAvatarChangedEvent(Id, oldAvatarUrl, AvatarURL));

                return true;
            }

            return false;
        }

        public bool ChangeBio(string? bio)
        {
            bio = string.IsNullOrWhiteSpace(bio) ? null : bio.Trim();

            if (bio != null && bio.Length > MaxBioLength)
                throw new InvalidBioLengthException(MaxBioLength);

            if (bio == Bio)
                return false;

            var oldBio = Bio;
            Bio = bio;
            ChangeUpdateTime();

            _domainEvents.Add(new UserProfileUpdatedEvent(Id, nameof(Bio), oldBio, Bio));

            return true;
        }

        public void ConfirmEmail()
        {
            if (!EmailConfirmed)
            {
                EmailConfirmed = true;
                ChangeUpdateTime();

                _domainEvents.Add(new UserEmailConfirmedEvent(Id, Email!));
            }
        }

        public void RecordLogin()
        {
            LastLoginAt = DateTime.UtcNow;

            _domainEvents.Add(new UserLoginRecordedEvent(Id, LastLoginAt.Value));
        }

        public void Verify()
        {
            if (!IsVerified)
            {
                IsVerified = true;
                ChangeUpdateTime();

                _domainEvents.Add(new UserVerifiedEvent(Id, UserName!));
            }
        }

        public void UnVerify()
        {
            if (IsVerified)
            {
                IsVerified = false;
                ChangeUpdateTime();

                _domainEvents.Add(new UserUnverifiedEvent(Id, UserName!));
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

        private void ValidateConstructorInputs(string email, DateOnly birthDate, string userName)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new UserArgumentNullException(nameof(email));
            if (string.IsNullOrWhiteSpace(userName))
                throw new UserArgumentNullException(nameof(userName));

            if (!IsValidEmail(email))
                throw new InvalidEmailFormatException();
            if (!_userNameRegex.IsMatch(userName.Trim().ToLower()))
                throw new InvalidUsernameFormatException();
            if (!IsValidBirthDate(birthDate))
                throw new InvalidBirthDateException(MinimumRequiredAge);
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
