using PlanningPoker.Core.Enums;
using PlanningPoker.Core.ViewModels;

namespace PlanningPoker.UI
{
    public class StateContainer
    {
        private string _sessionId = null;
        public string SessionId
        {
            get => _sessionId;
            set { _sessionId = value; NotifyStateChanged(); }
        }

        private GameSessionViewModel _gameSession = null;
        public GameSessionViewModel GameSession
        {
            get => _gameSession;
            set { _gameSession = value; NotifyStateChanged(); }
        }

        private string _localPlayerName = null;
        public string LocalPlayerName
        {
            get => _localPlayerName;
            set { _localPlayerName = value; NotifyStateChanged(); }
        }

        private DateTime _startedAt = DateTime.MinValue;
        public DateTime StartedAt
        {
            get => _startedAt;
            set { _startedAt = value; NotifyStateChanged(); }
        }

        public bool Started => StartedAt != DateTime.MinValue;

        public event Action OnChange;

        public void Reset()
        {
            _sessionId = null;
            _gameSession = null;
            _localPlayerName = null;
            _startedAt = DateTime.MinValue;
            NotifyStateChanged();
        }

        private void NotifyStateChanged()
        {
            Console.WriteLine("State changed. Has listeners: {0}", OnChange != null);
            OnChange?.Invoke();
        }

        public IEnumerable<SummaryViewModel> SummaryData
        {
            get
            {
                if (GameSession == null)
                    return default;

                Console.WriteLine("Calculating summary");

                var summaryData = GameSession.Players
                    .Where(w => w.LastMove != null)?
                    .GroupBy(g => g.LastMove.Number)
                    .Select(x => new SummaryViewModel(x.Key, x.Count().ToString()))
                    .OrderByDescending(x => x.Votes)
                    .ToList();

                if (summaryData?.Count == 0)
                    return new List<SummaryViewModel>()
                        {
                           new SummaryViewModel (CardNumber.Zero, "Média")
                        };

                var average = (int)(summaryData?.Average(x => (int)x.Number) ?? 0);
                Console.WriteLine("Average: {0}", average);

                summaryData.Add(new SummaryViewModel(Enum.Parse<CardNumber>(average.ToString()), "Média"));

                return summaryData;
            }
        }
    }
}
