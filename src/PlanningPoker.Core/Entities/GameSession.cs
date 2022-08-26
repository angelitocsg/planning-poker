using Newtonsoft.Json;
using PlanningPoker.Core.Enums;
using PlanningPoker.Core.Interfaces;

namespace PlanningPoker.Core.Entities
{
    public class GameSession : IGameSession<Player>
    {
        public string Id { get; private set; }
        public Player Owner { get; private set; }

        private List<Player> _players { get; set; }
        public IEnumerable<Player> Players { get => _players; }

        public bool Active { get; private set; }

        public DateTime CreatedAt { get; private set; }

        public DateTime? StartedAt { get; private set; }

        public DateTime? StopedAt { get; private set; }

        private List<string> _errorMesasges { get; set; }
        public IEnumerable<string> ErrorMessages { get => _errorMesasges; }

        public bool HasError { get => ErrorMessages != null && ErrorMessages?.Count() > 0; }

        private bool NotStarted { get => StartedAt == null && StopedAt == null; }

        private bool Running { get => StartedAt != null && StopedAt == null; }
        private bool Ended { get => StartedAt != null && StopedAt != null; }

        private GameSession() : this(string.Empty) { }

        public GameSession(string ownerName)
        {
            Id = Guid.NewGuid().ToString();
            Owner = new Player(ownerName);
            CreatedAt = DateTime.Now;

            _players = new List<Player>() { Owner };
            _errorMesasges = new List<string>();


            if (ownerName.Length > 15)
            {
                _errorMesasges.Add($"Owner name {ownerName} must contain up to 15 characters.");
                return;
            }
        }

        public static GameSession CreateGameSessionError(string message)
        {
            var gameSession = new GameSession();

            gameSession._errorMesasges.Add(message);

            return gameSession;
        }

        public void AddPlayer(string playerName)
        {
            if (Running || Ended)
            {
                _errorMesasges.Add($"Session {Id} is running or ended, {playerName} can't join now.");
                return;
            }

            if (playerName.Length > 15)
            {
                _errorMesasges.Add($"Player name {playerName} must contain up to 15 characters.");
                return;
            }

            var player = new Player(playerName);

            if (!_players.Any(x => x.Name == player.Name))
                _players.Add(player);
        }

        public void Start(string playerName)
        {
            if (Running)
            {
                _errorMesasges.Add($"Session {Id} already started.");
                return;
            }

            if (Owner.Name != playerName)
            {
                _errorMesasges.Add($"Only session owner {Owner.Name} can start session.");
                return;
            }

            if (NotStarted || Ended)
            {
                StartedAt = DateTime.Now;
                StopedAt = null;
            }
        }

        public void Stop(string playerName)
        {
            if (Ended)
            {
                _errorMesasges.Add($"Session {Id} already stopped.");
                return;
            }

            if (Owner.Name != playerName)
            {
                _errorMesasges.Add($"Only session owner {Owner.Name} can stop session.");
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
            if (!Running)
            {
                _errorMesasges.Add($"Session {Id} not running, {playerName} can't move yet.");
                return;
            }

            var player = _players.FirstOrDefault(x => x.Name == playerName);

            if (player == null)
            {
                _errorMesasges.Add($"{playerName} is not joined to session {Id}.");
                return;
            }

            player.SetLastMove(number);
        }
    }
}
