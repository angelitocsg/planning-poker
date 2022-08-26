using PlanningPoker.Core.Enums;
using PlanningPoker.Core.Interfaces;

namespace PlanningPoker.Core.Entities
{
    public class Card : ICard
    {
        public CardNumber Number { get; private set; }

        public Card(CardNumber number)
        {
            Number = number;
        }
    }
}
