using Newtonsoft.Json;
using PlanningPoker.Core.Interfaces;

namespace PlanningPoker.Core.ViewModels
{
    public class GameSessionViewModel : IGameSession<PlayerViewModel>
    {
        public string Id { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public PlayerViewModel Owner { get; set; } = new PlayerViewModel(string.Empty);

        public IEnumerable<PlayerViewModel> Players { get; set; } = new List<PlayerViewModel>();

        public bool Active { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? StartedAt { get; set; }

        public DateTime? StopedAt { get; set; }

        public IEnumerable<string>? ErrorMessages { get; set; }

        public bool HasError { get => ErrorMessages != null && ErrorMessages?.Count() > 0; }

        public bool NotStarted { get; set; }

        public bool Running { get; set; }

        public bool Ended { get; set; }

        public override string ToString() => JsonConvert.SerializeObject(this);
    }
}
