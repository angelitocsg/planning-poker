namespace PlanningPoker.Core.Interfaces
{
    public interface IGameSession<TPlayer>
    {
        public string Id { get; }
        public TPlayer Owner { get; }

        public IEnumerable<TPlayer> Players { get; }

        public bool Active { get; }

        public DateTime CreatedAt { get; }

        public DateTime? StartedAt { get; }

        public DateTime? StopedAt { get; }

        public IEnumerable<string>? ErrorMessages { get; }

        public bool HasError { get; }
    }
}
