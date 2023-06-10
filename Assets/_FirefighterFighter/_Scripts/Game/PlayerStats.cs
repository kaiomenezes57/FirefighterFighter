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
        public void SetPlayersTable_ClientRpc(string player1Name, string player2Name)
        {
            _player1Name.text = player1Name;
            _player2Name.text = player2Name;
        }

        [ClientRpc]
        public void UpdatePlayerPoints_ClientRpc(int amount, ulong targetClient)
        {
            if (NetworkManager.Singleton.LocalClientId == targetClient) { NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<PlayerPoints>().TotalPoints = amount; }

            string amountText = amount.ToString();

            if (targetClient == 0)
            {
                _player1Points.text = amountText;
            }
            else
            {
                _player2Points.text = amountText;
            }
        }
    }
}