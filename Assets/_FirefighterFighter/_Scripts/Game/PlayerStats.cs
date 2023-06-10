using TMPro;
using Unity.Netcode;
using UnityEngine;

namespace FirefighterFighter.Game
{
    public class PlayerStats : NetworkBehaviour
    {
        [SerializeField] private TextMeshProUGUI _player1Name;
        [SerializeField] private TextMeshProUGUI _player2Name;

        [SerializeField] private TextMeshProUGUI _player1Points;
        [SerializeField] private TextMeshProUGUI _player2Points;

        [ClientRpc]
        public void SetPlayersLabel_ClientRpc(string player1Name, string player2Name)
        {
            _player1Name.text = player1Name;
            _player2Name.text = player2Name;
        }

        [ServerRpc(RequireOwnership = false)]
        public void UpdateVisual_ServerRpc()
        {
            UpdateVisual_ClientRpc(
                NetworkManager.ConnectedClients[0].PlayerObject.GetComponent<PlayerPoints>().TotalPoints.ToString(),
                NetworkManager.ConnectedClients[1].PlayerObject.GetComponent<PlayerPoints>().TotalPoints.ToString());
        }

        [ClientRpc]
        public void UpdateVisual_ClientRpc(string player1Points, string player2Points)
        {
            _player1Points.text = $"{player1Points} points";
            _player2Points.text = $"{player2Points} points";
        }
    }
}