using PlanningPoker.Core.ViewModels;

namespace PlanningPoker.UI
{
    public class StateContainer
    {
        public string SessionId { get; private set; }

        public GameSessionViewModel _gameSession = null;
        public GameSessionViewModel GameSession
        {
            get => _gameSession;
            set
            {
                _gameSession = value;
                NotifyStateChanged();
            }
        }

        public string _localPlayerName = null;
        public string LocalPlayerName
        {
            get => _localPlayerName;
            set
            {
                _localPlayerName = value;
                NotifyStateChanged();
            }
        }

        private DateTime _startedAt = DateTime.MinValue;
        public DateTime StartedAt
        {
            get => _startedAt;
            set
            {
                _startedAt = value;
                NotifyStateChanged();
            }
        }

        public bool Started => StartedAt != DateTime.MinValue;


        public event Action OnChange;

        private void NotifyStateChanged() => OnChange?.Invoke();
    }
}
