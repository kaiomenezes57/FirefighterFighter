using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;

namespace FirefighterFighter.Networking
{
    public partial class LobbyManager : MonoBehaviour
    {
        private Lobby _hostLobby;
        private Lobby _clientLobby;
        private bool _startedGame;

        private LobbyVisual _visual;

        private async void Start()
        {
            _visual = FindObjectOfType<LobbyVisual>();
            await UnityServices.InitializeAsync();
        }

        public async void OnCreateLobby()
        {
            await Authenticate();
            CreateLobbyOptions options = new() 
            { 
                Player = GetPlayer(),
                Data = new Dictionary<string, DataObject>
                {
                    { "StartGame", new DataObject(DataObject.VisibilityOptions.Public, "0") }
                }
            };

            _hostLobby = await Lobbies.Instance.CreateLobbyAsync(lobbyName: "Lobby do Kaio", maxPlayers: 4, options);
            _clientLobby = _hostLobby;
            
            InvokeRepeating(nameof(SendLobbyHeartBeat), 5f, 5f);
            await UpdateLobby();
            
            _visual.ShowPlayersInLobbyScreen(_clientLobby);
            _visual.GoToLobbyRoom(true, _hostLobby.LobbyCode);
            Debug.Log($"{_hostLobby.LobbyCode}");
        }

        public async void OnJoinLobby(string joinCode)
        {
            if (!string.IsNullOrEmpty(joinCode))
            {
                await Authenticate();
                JoinLobbyByCodeOptions options = new() { Player = GetPlayer() };

                _clientLobby = await Lobbies.Instance.JoinLobbyByCodeAsync(joinCode, options);
                InvokeRepeating(nameof(CheckForUpdates), 3f, 3f);

                _visual.ShowPlayersInLobbyScreen(_clientLobby);
                _visual.GoToLobbyRoom(false, _clientLobby.LobbyCode);
                return;
            }

            Debug.LogError($"Invalid join code");
        }

        [ContextMenu("LobbyList")]
        public async void OnOpenLobbyList()
        {
            QueryLobbiesOptions queryOptions = new()
            {
                SampleResults = false,
                Filters = new List<QueryFilter>
                {
                        new QueryFilter(field: QueryFilter.FieldOptions.AvailableSlots, op: QueryFilter.OpOptions.GT, value: "0")
                },
                Order = new List<QueryOrder> { new QueryOrder(true, QueryOrder.FieldOptions.Created), }
            };

            QueryResponse response = await LobbyService.Instance.QueryLobbiesAsync(queryOptions);
            List<Lobby> lobbies = response.Results;
            _visual.GoToLobbyList(lobbies);
        }

        [ContextMenu("QuickLobby")]
        public async void OnQuickJoin()
        {
            try
            {
                await Authenticate();
                QuickJoinLobbyOptions options = new()
                {
                    Filter = new List<QueryFilter>() { new QueryFilter(field: QueryFilter.FieldOptions.MaxPlayers, op: QueryFilter.OpOptions.GE, value: "4") }
                };

                Lobby lobby = await LobbyService.Instance.QuickJoinLobbyAsync(options);
            }
            catch (LobbyServiceException e)
            {
                Debug.Log(e);
            }
        }

        [ContextMenu("LeaveLobby")]
        public async void OnLeaveLobby()
        {
            string clientId = AuthenticationService.Instance.PlayerId;
            await LobbyService.Instance.RemovePlayerAsync(_clientLobby.Id, clientId);
            _visual.GoToMainScreen();
        }

        public async void OnStartGame()
        {
            _clientLobby = await RelayManager.StartGame(_clientLobby);
            _visual.StartGame();

            CancelInvoke();
        }
    }


    /// <summary>
    /// LobbyManager partial 2
    /// Lobby Manager utilities.
    /// </summary>
    public partial class LobbyManager
    {
        private async Task Authenticate()
        {
            if (AuthenticationService.Instance.IsSignedIn) { return; }

            AuthenticationService.Instance.ClearSessionToken();
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }

        private Player GetPlayer()
        {
            Player player = new()
            {
                Data = new Dictionary<string, PlayerDataObject>
                {
                    { "name", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Public, _visual.GetPlayerNameInput()) }
                }
            };

            return player;
        }

        private async void CheckForUpdates()
        {
            if (_clientLobby == null || _startedGame) { return; }

            _ = UpdateLobby();
            _visual.ShowPlayersInLobbyScreen(_clientLobby);

            if (!_clientLobby.Data["StartGame"].Value.Equals("0"))
            {
                if (_hostLobby == null)
                {
                    await RelayManager.JoinRelay(_clientLobby.Data["StartGame"].Value);
                    _visual.StartGame();
                }

                _startedGame = true;
            }
        }

        private async void SendLobbyHeartBeat()
        {
            if (_hostLobby == null) { return; }
            await LobbyService.Instance.SendHeartbeatPingAsync(_hostLobby.Id);
            await UpdateLobby();
            _visual.ShowPlayersInLobbyScreen(_clientLobby);
        }

        public Lobby GetClientLobby()
        {
            return _clientLobby;
        }

        private async Task UpdateLobby()
        {
            if (_clientLobby == null) { return; }
            _clientLobby = await LobbyService.Instance.GetLobbyAsync(_clientLobby.Id);
        }
    }
}