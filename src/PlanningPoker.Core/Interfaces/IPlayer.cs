namespace PlanningPoker.Core.Interfaces
{
    public interface IPlayer<TCard>
    {
        public string Name { get; }
        public TCard? LastMove { get; }
    }
}
