using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using PlanningPoker.Core.Entities;
using PlanningPoker.Core.Enums;

namespace PlanningPoker.Bff
{
    public class GameHub : Hub
    {
        private readonly IMemoryCache _memoryCache;

        public GameHub(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }

        #region Private methods

        private static void Log(object data) => Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss:fff}] {JsonConvert.SerializeObject(data)}");

        private GameSession UpdateCache(GameSession gameSession)
        {
            return _memoryCache.GetOrCreate(gameSession.Id, entry =>
            {
                Log(string.Format("Updating session {0} cache.", gameSession.Id));

                entry.SetSlidingExpiration(TimeSpan.FromMinutes(30));
                entry.SetAbsoluteExpiration(TimeSpan.FromMinutes(30));

                return gameSession;
            });
        }

        private GameSession GetGameSessionFromCache(string sessionId)
        {
            if (!_memoryCache.TryGetValue(sessionId, out GameSession gameSession))
            {
                var gameSessionError = GameSession.CreateGameSessionError($"Error getting session {sessionId} from cache.");
                return gameSessionError;
            }

            if (string.IsNullOrWhiteSpace(gameSession.Owner.Name))
            {
                _memoryCache.Remove(sessionId);
                var gameSessionError = GameSession.CreateGameSessionError($"Invalid session {sessionId} from cache.");
                return gameSessionError;
            }

            return gameSession;
        }

        private async Task NotifyChangesToPlayers(GameSession gameSession)
        {
            gameSession = UpdateCache(gameSession);
            await Groups.AddToGroupAsync(Context.ConnectionId, gameSession.Id);
            await Clients.Group(gameSession.Id).SendAsync(GameHubListeners.SessionContent, gameSession.ToString());
        }

        private async Task NotifySessionCreatedToCaller(GameSession gameSession)
        {
            gameSession = UpdateCache(gameSession);
            await Groups.AddToGroupAsync(Context.ConnectionId, gameSession.Id);
            await Clients.Caller.SendAsync(GameHubListeners.SessionContent, gameSession.ToString());
        }

        private async Task NotifyErrorToCaller(GameSession gameSession)
        {
            Log(gameSession);

            if (!gameSession.HasError)
                return;

            await Clients.Caller.SendAsync(GameHubListeners.SessionError, gameSession.ToString());
        }

        #endregion

        public async Task CreateSession(string ownerName)
        {
            Log($"{nameof(CreateSession)}-{nameof(ownerName)}: {ownerName}");

            var gameSession = new GameSession(ownerName);
            await NotifySessionCreatedToCaller(gameSession);
            await NotifyErrorToCaller(gameSession);
        }

        public bool HasSession(string sessionId)
        {
            Log($"{nameof(HasSession)}-{nameof(sessionId)}");
            var gameSession = GetGameSessionFromCache(sessionId);
            return !gameSession.HasError;
        }

        public async Task JoinSession(string sessionId, string playerName)
        {
            Log($"{nameof(JoinSession)}-{nameof(sessionId)}: {sessionId} | {nameof(playerName)}: {playerName}");

            var gameSession = GetGameSessionFromCache(sessionId);
            gameSession.AddPlayer(playerName);

            await NotifyChangesToPlayers(gameSession);
            await NotifyErrorToCaller(gameSession);
        }

        public async Task SelectCardNumber(string sessionId, string playerName, CardNumber number)
        {
            Log($"{nameof(SelectCardNumber)}-{nameof(sessionId)}: {sessionId} | {nameof(playerName)}: {playerName} | {nameof(number)}: {number} ");

            var gameSession = GetGameSessionFromCache(sessionId);
            gameSession.SetPlayerMove(playerName, number);

            await NotifyChangesToPlayers(gameSession);
            await NotifyErrorToCaller(gameSession);
        }

        public async Task UpdateDescription(string sessionId, string playerName, string description)
        {
            Log($"{nameof(UpdateDescription)}-{nameof(sessionId)}: {sessionId} | {nameof(playerName)}: {playerName} | {nameof(description)}: {description}");

            var gameSession = GetGameSessionFromCache(sessionId);
            gameSession.UpdateDescription(playerName, description);

            await NotifyChangesToPlayers(gameSession);
            await NotifyErrorToCaller(gameSession);
        }

        public async Task StartSession(string sessionId, string playerName)
        {
            Log($"{nameof(StartSession)}-{nameof(sessionId)}: {sessionId} | {nameof(playerName)}: {playerName}");

            var gameSession = GetGameSessionFromCache(sessionId);
            gameSession.Start(playerName);

            await NotifyChangesToPlayers(gameSession);
            await NotifyErrorToCaller(gameSession);
        }

        public async Task RestartSession(string sessionId, string playerName)
        {
            Log($"{nameof(RestartSession)}-{nameof(sessionId)}: {sessionId} | {nameof(playerName)}: {playerName}");

            var gameSession = GetGameSessionFromCache(sessionId);
            gameSession.Restart(playerName);

            await NotifyChangesToPlayers(gameSession);
            await NotifyErrorToCaller(gameSession);
        }

        public async Task StopSession(string sessionId, string playerName)
        {
            Log($"{nameof(StopSession)}-{nameof(sessionId)}: {sessionId} | {nameof(playerName)}: {playerName}");

            var gameSession = GetGameSessionFromCache(sessionId);
            gameSession.Stop(playerName);

            await NotifyChangesToPlayers(gameSession);
            await NotifyErrorToCaller(gameSession);
        }
    }
}