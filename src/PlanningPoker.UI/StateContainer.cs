﻿using PlanningPoker.Core.Enums;
using PlanningPoker.Core.ViewModels;

namespace PlanningPoker.UI
{
    public class StateContainer
    {
        private string _sessionId = null;
        public string SessionId
        {
            get => _sessionId; set
            {
                _sessionId = value;
                NotifyStateChanged();
            }
        }

        private GameSessionViewModel _gameSession = null;
        public GameSessionViewModel GameSession
        {
            get => _gameSession;
            set
            {
                _gameSession = value;
                NotifyStateChanged();
            }
        }

        private string _localPlayerName = null;
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

        public void Reset()
        {
            _sessionId = null;
            _gameSession = null;
            _localPlayerName = null;
            _startedAt = DateTime.MinValue;
        }

        private void NotifyStateChanged()
        {
            Console.WriteLine("State changed. OnChange {0}", OnChange != null);
            OnChange?.Invoke();
        }

        public IEnumerable<(CardNumber Number, string Votes)> SummaryData
        {
            get
            {
                Console.WriteLine("Change summary");

                if (GameSession == null)
                    return default;

                var summaryData = GameSession.Players
                    .Where(w => w.LastMove != null)
                    .GroupBy(g => g.LastMove.Number)
                    .Select(x => (Number: x.Key, Votes: x.Count().ToString()))
                    .OrderBy(x => x.Votes)
                    .ToList();

                var average = (int)summaryData?.Average(x => (int)x.Number);
                summaryData.Add((Number: Enum.Parse<CardNumber>(average.ToString()), Votes: "Média"));

                return summaryData;
            }
        }
    }
}