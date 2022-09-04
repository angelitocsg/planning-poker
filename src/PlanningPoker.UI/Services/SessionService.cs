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
        Task StopSession();
        Task<bool> ValidSession(string pSessionId);
        Task<bool> HasSession();
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
                Console.WriteLine("Conexão perdida: {0}", error?.Message);
                await Task.Delay(new Random().Next(0, 5) * 1000);
                await Connection.StartAsync();
            };

            Connection.Reconnecting += error =>
            {
                Console.WriteLine("Reconectando... {0}", error?.Message);
                Debug.Assert(Connection.State == HubConnectionState.Reconnecting);
                return Task.CompletedTask;
            };

            Connection.Reconnected += connectionId =>
            {
                Console.WriteLine("Reconectado! {0}", connectionId);
                Debug.Assert(Connection.State == HubConnectionState.Connected);
                return Task.CompletedTask;
            };

            RegisterListeners();
        }

        private void RegisterListeners()
        {
            Connection.On<string>(GameHubListeners.SessionContent, async (value) =>
            {
                Console.WriteLine("SessionContent received.");

                if (string.IsNullOrEmpty(value))
                    return;

                var content = JsonConvert.DeserializeObject<GameSessionViewModel>(value);

                _stateContainer.GameSession = content;

                Console.WriteLine(JsonConvert.SerializeObject(content));

                await _localStorage.SetItemAsync("SessionId", _stateContainer.GameSession.Id);
            });

            Connection.On<string>(GameHubListeners.SessionError, (value) =>
            {
                Console.WriteLine("SessionError received.");

                if (string.IsNullOrEmpty(value))
                    return;

                Console.WriteLine(JsonConvert.SerializeObject(value));
            });
        }

        public async Task CreateSession(string ownerName)
        {
            Console.WriteLine("SessionService.CreateSession: {0}", ownerName);
            await Connect();
            await Connection.InvokeAsync(GameHubActions.CreateSession, ownerName);
            await _localStorage.SetItemAsync("PlayerName", ownerName);
            _stateContainer.LocalPlayerName = ownerName;
        }

        public async Task JoinSession(string sessionId, string playerName)
        {
            Console.WriteLine("SessionService.JoinSession: {0}-{1}", sessionId, playerName);
            await Connect();
            await Connection.InvokeAsync(GameHubActions.JoinSession, sessionId, playerName);
            await _localStorage.SetItemAsync("PlayerName", playerName);
            _stateContainer.LocalPlayerName = playerName;
        }

        public async Task StartSession()
        {
            var playerName = await GetPlayerName();
            var sessionId = await GetSessionId();
            Console.WriteLine("SessionService.StartSession: {0}-{1}", sessionId, playerName);
            if (Connection == null) await Connect();
            await Connection.InvokeAsync(GameHubActions.StartSession, sessionId, playerName);
        }

        public async Task StopSession()
        {
            var playerName = await GetPlayerName();
            var sessionId = await GetSessionId();
            Console.WriteLine("SessionService.StopSession: {0}-{1}", sessionId, playerName);
            if (Connection == null) await Connect();
            await Connection.InvokeAsync(GameHubActions.StopSession, sessionId, playerName);
        }

        public async Task<bool> HasSession()
        {
            var sessionId = await GetSessionId();
            Console.WriteLine("SessionService.HasSession: {0}", sessionId);

            if (string.IsNullOrWhiteSpace(sessionId)) return false;

            Console.WriteLine("continuou");
            Console.WriteLine(sessionId);

            if (Connection == null) await Connect();
            var hasSession = await Connection.InvokeAsync<bool>(GameHubActions.HasSession, sessionId);

            if (!hasSession)
            {
                await ClearSession();
                _stateContainer.Reset();
            }

            return hasSession;
        }

        public async Task SelectCardNumber(CardNumber number)
        {
            var playerName = await GetPlayerName();
            var sessionId = await GetSessionId();
            Console.WriteLine("SessionService.SelectCardNumber: {0}-{1}", sessionId, playerName);
            if (Connection == null) await Connect();
            await Connection.InvokeAsync(GameHubActions.SelectCardNumber, sessionId, playerName, number);
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

            if (!string.IsNullOrWhiteSpace(playerName))
                _stateContainer.LocalPlayerName = playerName;

            if (!string.IsNullOrWhiteSpace(sessionId))
                _stateContainer.SessionId = sessionId;

            return !(string.IsNullOrWhiteSpace(playerName)
                || string.IsNullOrWhiteSpace(sessionId)
                || pSessionId != sessionId);
        }
    }
}
