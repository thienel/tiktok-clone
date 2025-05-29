using TikTokClone.Domain.Entities;

namespace TikTokClone.Domain.Event
{
    public record UserCreatedEvent(User User) : IDomainEvent
    {
        public DateTime OccurredOn { get; } = DateTime.UtcNow;
    }

    public record UserNameChangedEvent(User User, string? OldUserName = null, string? NewUserName = null) : IDomainEvent
    {
        public DateTime OccurredOn { get; } = DateTime.UtcNow;
    }

    public record UserEmailConfirmedEvent(User User) : IDomainEvent
    {
        public DateTime OccurredOn { get; } = DateTime.UtcNow;
    }

    public record UserVerifiedEvent(User User) : IDomainEvent
    {
        public DateTime OccurredOn { get; } = DateTime.UtcNow;
    }

    public record UserUnverifiedEvent(User User) : IDomainEvent
    {
        public DateTime OccurredOn { get; } = DateTime.UtcNow;
    }

    public record UserAvatarChangedEvent(User User, string? NewAvatarUrl) : IDomainEvent
    {
        public DateTime OccurredOn { get; } = DateTime.UtcNow;
    }

    public record UserBioChangedEvent(User User, string? NewBio) : IDomainEvent
    {
        public DateTime OccurredOn { get; } = DateTime.UtcNow;
    }
}
