
namespace AuthService.Domain.Event
{
    public interface IDomainEvent
    {
        DateTime OccurredOn { get; }
        Guid EventId { get; }
    }

}
