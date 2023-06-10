using FirefighterFighter.Networking;
using System.Collections;
using Unity.Netcode;
using Unity.VisualScripting;

public class InitialServerSetup : NetworkBehaviour
{
    private LobbyManager _lobbyManager;

    private IEnumerator Start()
    {
        _lobbyManager = FindObjectOfType<LobbyManager>();

        if (IsServer)
        {
            while (NetworkManager.Singleton.ConnectedClients.Count != _lobbyManager.GetClientLobby().Players.Count)
            {
                yield return null;
            }

            for (int i = 0; i < NetworkManager.Singleton.ConnectedClients.Count; i++)
            {
                AddUserData(i);
                SetFloatingName(i);
            }

            SetPlayerTable();
        }
    }

    private void AddUserData(int index)
    {
        UserData userData = NetworkManager.Singleton.ConnectedClients[(ulong)index].PlayerObject.AddComponent<UserData>();
        userData.SetPlayer(_lobbyManager.GetClientLobby().Players[index]);
    }

    private void SetFloatingName(int index)
    {
        NetworkManager.Singleton.ConnectedClients[(ulong)index].PlayerObject.GetComponentInChildren<PlayerName>().
            SetPlayerNameClientRpc(_lobbyManager.GetClientLobby().Players[index].Data["name"].Value);
    }

    private void SetPlayerTable()
    {
        string player1Name = _lobbyManager.GetClientLobby().Players[0].Data["name"].Value;
        string player2Name = _lobbyManager.GetClientLobby().Players[1].Data["name"].Value;
        FindObjectOfType<PlayerHud>().SetPlayersTableClientRpc(player1Name, player2Name);
    }
}
