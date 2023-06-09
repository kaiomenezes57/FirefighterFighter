using FirefighterFighter.Networking;
using System.Collections;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class PlayerName : NetworkBehaviour
{
    [SerializeField] private TextMeshProUGUI _playerName;

    private IEnumerator Start()
    {
        LobbyManager lobbyManager = FindObjectOfType<LobbyManager>();

        if (IsServer)
        {
            while (NetworkManager.Singleton.ConnectedClients.Count != lobbyManager.GetClientLobby().Players.Count)
            {
                yield return null;
            }
        }

        for (int i = 0; i < NetworkManager.Singleton.ConnectedClients.Count; i++)
        {
            NetworkManager.Singleton.ConnectedClients[(ulong)i].PlayerObject.GetComponentInChildren<PlayerName>().
                SetPlayerNameClientRpc(lobbyManager.GetClientLobby().Players[i].Data["name"].Value);
        }
    }

    [ClientRpc]
    public void SetPlayerNameClientRpc(string playerName)
    {
        _playerName.text = playerName;
    }

    private void Update()
    {
        transform.LookAt(Camera.main.transform);
    }
}