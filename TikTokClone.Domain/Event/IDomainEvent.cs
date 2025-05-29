
namespace TikTokClone.Domain.Event
{
    public interface IDomainEvent
    {
        DateTime OccurredOn { get; }
    }
}
