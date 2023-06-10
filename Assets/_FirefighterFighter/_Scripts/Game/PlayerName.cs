using TMPro;
using Unity.Netcode;
using UnityEngine;

namespace FirefighterFighter.Game
{
    public class PlayerName : NetworkBehaviour
    {
        [SerializeField] private TextMeshProUGUI _playerName;

        private void Update()
        {
            transform.LookAt(UnityEngine.Camera.main.transform);
        }

        [ClientRpc]
        public void SetPlayerNameClientRpc(string playerName)
        {
            _playerName.text = playerName;
        }
    }
}