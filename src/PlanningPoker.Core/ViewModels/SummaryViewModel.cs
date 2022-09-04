using PlanningPoker.Core.Enums;

namespace PlanningPoker.Core.ViewModels
{
    public class SummaryViewModel
    {
        public SummaryViewModel(CardNumber number, string votes)
        {
            Number = number;
            Votes = votes;
        }

        public CardNumber Number { get; set; }
        public string Votes { get; set; }
    }
}
