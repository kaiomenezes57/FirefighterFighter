using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode.Transports.UTP;
using Unity.Netcode;
using Unity.Networking.Transport.Relay;
using Unity.Services.Lobbies.Models;
using Unity.Services.Lobbies;
using Unity.Services.Relay.Models;
using Unity.Services.Relay;

namespace FirefighterFighter.Networking
{
    public static class RelayManager
    {
        private static async Task<string> CreateRelay()
        {
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(maxConnections: 2);
            string joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
            RelayServerData serverData = new(allocation, "dtls");

            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(serverData);
            NetworkManager.Singleton.StartHost();

            return joinCode;
        }

        public static async Task JoinRelay(string joinCode)
        {
            JoinAllocation allocation = await RelayService.Instance.JoinAllocationAsync(joinCode);
            RelayServerData serverData = new(allocation, "dtls");

            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(serverData);
            NetworkManager.Singleton.StartClient();
        }

        public static async Task<Lobby> StartGame(Lobby clientLobby)
        {
            string joinCode = await CreateRelay();

            Lobby lobby = await Lobbies.Instance.UpdateLobbyAsync(clientLobby.Id, new UpdateLobbyOptions
            {
                Data = new Dictionary<string, DataObject>
                {
                    { "StartGame", new DataObject(DataObject.VisibilityOptions.Public, joinCode) }
                }
            });

            clientLobby = lobby;
            return clientLobby;
        }
    }
}