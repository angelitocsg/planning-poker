using PlanningPoker.Core.Interfaces;

namespace PlanningPoker.Core.ViewModels
{
    public class PlayerViewModel : IPlayer<CardViewModel>
    {
        public PlayerViewModel(string name)
        {
            Name = name;
        }

        public string Name { get; set; }
        public CardViewModel? LastMove { get; set; }
    }
}
