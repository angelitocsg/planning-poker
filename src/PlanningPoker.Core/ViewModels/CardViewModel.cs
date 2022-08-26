using PlanningPoker.Core.Enums;
using PlanningPoker.Core.Interfaces;

namespace PlanningPoker.Core.ViewModels
{
    public class CardViewModel : ICard
    {
        public CardNumber Number { get; set; }
    }
}
