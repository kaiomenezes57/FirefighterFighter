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
    public class LobbyManager : MonoBehaviour
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

        private async Task Authenticate()
        {
            if (AuthenticationService.Instance.IsSignedIn) { return; }

            AuthenticationService.Instance.ClearSessionToken();
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }

        public async void CreateLobby()
        {
            await Authenticate();
            CreateLobbyOptions options = new() 
            { 
                Player = GetPlayer(),
                Data = new Dictionary<string, DataObject>
                {
                    { "StartGame", new DataObject(DataObject.VisibilityOptions.Member, "0") }
                }
            };

            _hostLobby = await Lobbies.Instance.CreateLobbyAsync(lobbyName: "Room 01", maxPlayers: 4, options);
            _clientLobby = _hostLobby;
            
            InvokeRepeating(nameof(SendLobbyHeartBeat), 10f, 10f);
            await UpdateLobby();
            
            _visual.ShowPlayersInLobbyScreen(_clientLobby);
            _visual.GoToLobbyRoom(true, _hostLobby.LobbyCode);
        }

        public async void JoinLobbyByCode()
        {
            await Authenticate();
            JoinLobbyByCodeOptions options = new() { Player = GetPlayer() };

            _clientLobby = await Lobbies.Instance.JoinLobbyByCodeAsync(_visual.GetLobbyCodeInput(), options);
            InvokeRepeating(nameof(CheckForUpdates), 3f, 3f);

            _visual.ShowPlayersInLobbyScreen(_clientLobby);
            _visual.GoToLobbyRoom(false, _clientLobby.LobbyCode);
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

        private void CheckForUpdates()
        {
            if (_clientLobby == null || _startedGame) { return; }

            _ = UpdateLobby();
            _visual.ShowPlayersInLobbyScreen(_clientLobby);

            if (!_clientLobby.Data["StartGame"].Value.Equals("0"))
            {
                if (_hostLobby == null)
                {
                    JoinRelay(_clientLobby.Data["StartGame"].Value);
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

        public async Task<string> CreateRelay()
        {
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(maxConnections: 2);
            string joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
            RelayServerData serverData = new(allocation, "dtls");

            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(serverData);
            NetworkManager.Singleton.StartHost();

            return joinCode;
        }

        public async void JoinRelay(string joinCode)
        {
            JoinAllocation allocation = await RelayService.Instance.JoinAllocationAsync(joinCode);
            RelayServerData serverData = new(allocation, "dtls");

            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(serverData);
            NetworkManager.Singleton.StartClient();

            _visual.StartGame();
        }

        public async void StartGame()
        {
            string joinCode = await CreateRelay();

            Lobby lobby = await Lobbies.Instance.UpdateLobbyAsync(_clientLobby.Id, new UpdateLobbyOptions
            {
                Data = new Dictionary<string, DataObject>
                {
                    { "StartGame", new DataObject(DataObject.VisibilityOptions.Member, joinCode) }
                }
            });

            _clientLobby = lobby;
            _visual.StartGame();
        }
    }
}