using PlanningPoker.Core.Enums;
using PlanningPoker.Core.Interfaces;

namespace PlanningPoker.Core.Entities
{
    public class Player : IPlayer<Card>
    {
        public Player(string name)
        {
            Name = name;
        }

        public string Name { get; private set; }
        public Card? LastMove { get; private set; }

        internal void SetLastMove(CardNumber number)
        {
            LastMove = new Card(number);
        }
    }
}
