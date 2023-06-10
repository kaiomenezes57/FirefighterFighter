using TMPro;
using Unity.Netcode;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class PlayerHud : NetworkBehaviour
{
    [SerializeField] private TextMeshProUGUI _player1Name;
    [SerializeField] private TextMeshProUGUI _player2Name;

    [SerializeField] private TextMeshProUGUI _player1Points;
    [SerializeField] private TextMeshProUGUI _player2Points;

    public void InitialSet(Player player)
    {
        Debug.Log($"{player.Data["name"].Value}");
    }

    [ClientRpc]
    public void SetPlayersTableClientRpc(string player1Name, string player2Name)
    {
        _player1Name.text = player1Name;
        _player2Name.text = player2Name;
    }
}
