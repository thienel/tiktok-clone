namespace TikTokClone.Domain.Event
{
    public abstract class DomainEvent : IDomainEvent
    {
        public DateTime OccurredOn { get; }
        public Guid EventId { get; }

        protected DomainEvent()
        {
            EventId = Guid.NewGuid();
            OccurredOn = DateTime.UtcNow;
        }
    }
}
