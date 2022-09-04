using Blazored.LocalStorage;
using Microsoft.AspNetCore.SignalR.Client;
using Newtonsoft.Json;
using PlanningPoker.Core.Enums;
using PlanningPoker.Core.ViewModels;
using System.Diagnostics;

namespace PlanningPoker.UI.Services
{
    public interface ISessionService
    {
        Task CreateSession(string ownerName);
        Task JoinSession(string sessionId, string playerName);
        Task StartSession();
        Task RestartSession();
        Task StopSession();
        Task UpdateDescription(string description);
        Task<bool> ValidSession(string pSessionId);
        Task<bool> HasSession(string pSessionId = null);
        Task SelectCardNumber(CardNumber number);
    }

    public class SessionService : ISessionService
    {
        private readonly string _backendUrl;
        private readonly ILocalStorageService _localStorage;
        private readonly StateContainer _stateContainer;

        private HubConnection Connection { get; set; }

        public SessionService(IConfiguration configuration,
            ILocalStorageService localStorage, StateContainer stateContainer)
        {
            _backendUrl = configuration["backendUrl"];
            _localStorage = localStorage;
            _stateContainer = stateContainer;
        }

        private async Task Connect()
        {
            Connection = new HubConnectionBuilder()
               .WithUrl(_backendUrl)
               .WithAutomaticReconnect()
               .Build();

            await Connection.StartAsync();

            Connection.Closed += async (error) =>
            {
                Console.WriteLine("[Service] Conexão perdida: {0}", error?.Message);
                await Task.Delay(new Random().Next(0, 5) * 1000);
                await Connection.StartAsync();
            };

            Connection.Reconnecting += error =>
            {
                Console.WriteLine("[Service] Reconectando... {0}", error?.Message);
                Debug.Assert(Connection.State == HubConnectionState.Reconnecting);
                return Task.CompletedTask;
            };

            Connection.Reconnected += connectionId =>
            {
                Console.WriteLine("[Service] Reconectado! {0}", connectionId);
                Debug.Assert(Connection.State == HubConnectionState.Connected);
                return Task.CompletedTask;
            };

            RegisterListeners();
        }

        private void RegisterListeners()
        {
            Connection.On<string>(GameHubListeners.SessionContent, async (value) =>
            {
                Console.WriteLine("[Service] SessionContent received. HasValue: {0}", value != null);

                if (string.IsNullOrEmpty(value))
                    return;

                var content = JsonConvert.DeserializeObject<GameSessionViewModel>(value);

                _stateContainer.GameSession = content;

                Console.WriteLine(JsonConvert.SerializeObject(content));

                await _localStorage.SetItemAsync("SessionId", _stateContainer.GameSession.Id);
            });

            Connection.On<string>(GameHubListeners.SessionError, (value) =>
            {
                Console.WriteLine("[Service] SessionError received.");

                if (string.IsNullOrEmpty(value))
                    return;

                Console.WriteLine(JsonConvert.SerializeObject(value));

            });
        }

        public async Task CreateSession(string ownerName)
        {
            Console.WriteLine("[Service] CreateSession: {0}", ownerName);
            await Connect();
            await Connection.InvokeAsync(GameHubActions.CreateSession, ownerName);
            await _localStorage.SetItemAsync("PlayerName", ownerName);
            _stateContainer.LocalPlayerName = ownerName;
        }

        public async Task JoinSession(string sessionId, string playerName)
        {
            Console.WriteLine("[Service] JoinSession: {0}-{1}", sessionId, playerName);
            await Connect();
            await Connection.InvokeAsync(GameHubActions.JoinSession, sessionId, playerName);
            await _localStorage.SetItemAsync("PlayerName", playerName);
            _stateContainer.LocalPlayerName = playerName;
        }

        public async Task StartSession()
        {
            var playerName = await GetPlayerName();
            var sessionId = await GetSessionId();
            Console.WriteLine("[Service] StartSession: {0}-{1}", sessionId, playerName);
            if (Connection == null) await Connect();
            await Connection.InvokeAsync(GameHubActions.StartSession, sessionId, playerName);
        }

        public async Task RestartSession()
        {
            var playerName = await GetPlayerName();
            var sessionId = await GetSessionId();
            Console.WriteLine("[Service] RestartSession: {0}-{1}", sessionId, playerName);
            if (Connection == null) await Connect();
            await Connection.InvokeAsync(GameHubActions.RestartSession, sessionId, playerName);
        }

        public async Task StopSession()
        {
            var playerName = await GetPlayerName();
            var sessionId = await GetSessionId();
            Console.WriteLine("[Service] StopSession: {0}-{1}", sessionId, playerName);
            if (Connection == null) await Connect();
            await Connection.InvokeAsync(GameHubActions.StopSession, sessionId, playerName);
        }

        public async Task UpdateDescription(string description)
        {
            var playerName = await GetPlayerName();
            var sessionId = await GetSessionId();
            Console.WriteLine("[Service] UpdateDescription: {0}-{1}-{2}", sessionId, playerName, description);
            if (Connection == null) await Connect();
            await Connection.InvokeAsync(GameHubActions.UpdateDescription, sessionId, playerName, description);
        }

        public async Task SelectCardNumber(CardNumber number)
        {
            var playerName = await GetPlayerName();
            var sessionId = await GetSessionId();
            Console.WriteLine("[Service] SelectCardNumber: {0}-{1}", sessionId, playerName);
            if (Connection == null) await Connect();
            await Connection.InvokeAsync(GameHubActions.SelectCardNumber, sessionId, playerName, number);
        }

        public async Task<bool> HasSession(string pSessionId = null)
        {
            var sessionId = pSessionId ?? await GetSessionId();
            Console.WriteLine("[Service] HasSession: {0}|{1}", pSessionId, sessionId);

            if (string.IsNullOrWhiteSpace(sessionId)) return false;

            if (Connection == null) await Connect();
            var hasSession = await Connection.InvokeAsync<bool>(GameHubActions.HasSession, sessionId);

            if (!hasSession)
            {
                await ClearSession();
                _stateContainer.Reset();
            }

            Console.WriteLine("[Service] HasSession: {0}", hasSession);

            return hasSession;
        }

        public async Task<string> GetSessionId() => await _localStorage.GetItemAsync<string>("SessionId");
        public async Task<string> GetPlayerName() => await _localStorage.GetItemAsync<string>("PlayerName");

        public async Task ClearSession()
        {
            await _localStorage.RemoveItemAsync("SessionId");
            await _localStorage.RemoveItemAsync("PlayerName");
        }

        public async Task<bool> ValidSession(string pSessionId)
        {
            var playerName = await GetPlayerName();
            var sessionId = await GetSessionId();

            Console.WriteLine("[Service] ValidSession:\n SessionId {0}\n pSessionId {1}\n PlayerName {2}\n", sessionId, pSessionId, playerName);

            if (!string.IsNullOrWhiteSpace(playerName))
                _stateContainer.LocalPlayerName = playerName;

            if (!string.IsNullOrWhiteSpace(sessionId))
                _stateContainer.SessionId = sessionId;

            if (!string.IsNullOrWhiteSpace(pSessionId) && pSessionId != sessionId)
                await _localStorage.SetItemAsync("SessionId", pSessionId);

            var valid = !string.IsNullOrWhiteSpace(playerName) && !string.IsNullOrWhiteSpace(sessionId);

            Console.WriteLine("[Service] ValidSession: {0}", valid);

            return valid;
        }
    }
}
