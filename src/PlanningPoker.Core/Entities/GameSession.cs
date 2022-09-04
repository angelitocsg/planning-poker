using Newtonsoft.Json;
using PlanningPoker.Core.Enums;
using PlanningPoker.Core.Interfaces;

namespace PlanningPoker.Core.Entities
{
    public class GameSession : IGameSession<Player>
    {
        public string Id { get; private set; }

        public string Description { get; private set; } = string.Empty;

        public Player Owner { get; private set; }

        private List<Player> _players { get; set; }
        public IEnumerable<Player> Players { get => _players; }

        public bool Active { get; private set; }

        public DateTime CreatedAt { get; private set; }

        public DateTime? StartedAt { get; private set; }

        public DateTime? StopedAt { get; private set; }

        private List<string> _errorMessages { get; set; }
        public IEnumerable<string> ErrorMessages { get => _errorMessages; }

        public bool HasError { get => ErrorMessages != null && ErrorMessages?.Count() > 0; }

        public bool NotStarted { get => StartedAt == null && StopedAt == null; }
        public bool Running { get => StartedAt != null && StopedAt == null; }
        public bool Ended { get => StartedAt != null && StopedAt != null; }

        private GameSession() : this(string.Empty) { }

        public GameSession(string ownerName)
        {
            Id = Guid.NewGuid().ToString();
            Owner = new Player(ownerName);
            CreatedAt = DateTime.Now;

            _players = new List<Player>() { Owner };
            _errorMessages = new List<string>();


            if (ownerName.Length > 15)
            {
                _errorMessages.Add($"Owner name {ownerName} must contain up to 15 characters.");
                return;
            }
        }

        private void ClearErrors() => _errorMessages = new List<string>();

        private void ClearMoves()
        {
            _players = _players.Select(x =>
            {
                x.ResetMove();
                return x;
            }).ToList();
        }

        public static GameSession CreateGameSessionError(string message)
        {
            var gameSession = new GameSession();

            gameSession._errorMessages.Add(message);

            return gameSession;
        }

        public void AddPlayer(string playerName)
        {
            ClearErrors();

            if (playerName == "[GUEST]")
            {
                AddPlayerOrGuest(playerName);
                return;
            }

            if (Running)
            {
                _errorMessages.Add($"Session {Id} is running, {playerName} can't join now.");
                return;
            }

            if (playerName.Length > 15)
            {
                _errorMessages.Add($"Player name {playerName} must contain up to 15 characters.");
                return;
            }

            if (_players.Count >= 14)
            {
                _errorMessages.Add($"Maximum number of players reached.");
                return;
            }

            AddPlayerOrGuest(playerName);
        }

        private void AddPlayerOrGuest(string playerName)
        {
            ClearErrors();

            var player = new Player(playerName);

            if (!_players.Any(x => x.Name == player.Name))
                _players.Add(player);
        }

        public void Start(string playerName)
        {
            ClearErrors();
            ClearMoves();

            if (Running)
            {
                _errorMessages.Add($"Session {Id} already started.");
                return;
            }

            if (Owner.Name != playerName)
            {
                _errorMessages.Add($"Only session owner {Owner.Name} can start session. Received {playerName}");
                return;
            }

            if (NotStarted || Ended)
            {
                StartedAt = DateTime.Now;
                StopedAt = null;
            }
        }

        public void Restart(string playerName)
        {
            ClearErrors();
            ClearMoves();

            if (Running)
            {
                _errorMessages.Add($"Session {Id} already started.");
                return;
            }

            if (Owner.Name != playerName)
            {
                _errorMessages.Add($"Only session owner {Owner.Name} can restart session. Received {playerName}");
                return;
            }

            if (NotStarted || Ended)
            {
                Description = string.Empty;
                StartedAt = null;
                StopedAt = null;
            }
        }

        public void Stop(string playerName)
        {
            ClearErrors();

            if (Ended)
            {
                _errorMessages.Add($"Session {Id} already stopped.");
                return;
            }

            if (Owner.Name != playerName)
            {
                _errorMessages.Add($"Only session owner {Owner.Name} can stop session.");
                return;
            }

            if (Running)
            {
                StopedAt = DateTime.Now;
            }
        }

        public override string ToString() => JsonConvert.SerializeObject(this);

        public void SetPlayerMove(string playerName, CardNumber number)
        {
            ClearErrors();

            if (!Running)
            {
                _errorMessages.Add($"Session {Id} not running, {playerName} can't move yet.");
                return;
            }

            var player = _players.FirstOrDefault(x => x.Name == playerName);

            if (player == null)
            {
                _errorMessages.Add($"{playerName} is not joined to session {Id}.");
                return;
            }

            player.SetLastMove(number);
        }

        public void UpdateDescription(string playerName, string description)
        {
            ClearErrors();

            if (Running)
            {
                _errorMessages.Add($"Session {Id} already started, you can't update description.");
                return;
            }

            if (Owner.Name != playerName)
            {
                _errorMessages.Add($"Only session owner {Owner.Name} can update description. Received {playerName}");
                return;
            }

            Description = description;
        }
    }
}
