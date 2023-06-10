using TMPro;
using Unity.Netcode;
using UnityEngine;

public class PlayerName : NetworkBehaviour
{
    [SerializeField] private TextMeshProUGUI _playerName;

    private void Update()
    {
        transform.LookAt(Camera.main.transform);
    }

    [ClientRpc]
    public void SetPlayerNameClientRpc(string playerName)
    {
        _playerName.text = playerName;
    }

    [ClientRpc]
    public void SetPlayerTableClientRpc(string playerName, int playerPoints = 0)
    {
        Debug.Log($"Player name => {playerName}");
    }
}