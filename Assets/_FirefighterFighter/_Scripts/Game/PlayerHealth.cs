using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

namespace FirefighterFighter.Game
{
    public class PlayerHealth : NetworkBehaviour
    {
        [SerializeField] private Slider _healthSlider;
        private int _health = 100;
        public bool IsDied;
        public bool GameStarted;

        private void Start()
        {
            _healthSlider.maxValue = 100;
            _healthSlider.value = _health;
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (!GameStarted) { return; }

            if (collision.transform.GetComponentInParent<CityProp>() != null)
            {
                TakeDamage_ServerRpc(Random.Range(10, 20));
            }
        }

        [ClientRpc]
        public void EnableCollisionMode_ClientRpc()
        {
            GameStarted = true;
        }

        [ServerRpc(RequireOwnership = false)]
        public void TakeDamage_ServerRpc(int damage, ServerRpcParams serverRpcParams = default)
        {
            ulong clientID = serverRpcParams.Receive.SenderClientId;
            for (int i = 0; i < NetworkManager.Singleton.ConnectedClients.Count; i++)
            {
                if (NetworkManager.Singleton.ConnectedClients[(ulong)i].ClientId == clientID)
                {
                    NetworkManager.Singleton.ConnectedClients[(ulong)i].PlayerObject.
                        GetComponent<PlayerHealth>().TakeDamage_ClientRpc(damage);
                }
            }
        }

        [ClientRpc]
        public void TakeDamage_ClientRpc(int damage)
        {
            _health -= damage;
            _healthSlider.value = _health;

            if (_health <= 0)
            {
                IsDied = true;
                GetComponent<PrometeoCarController>().enabled = false;
                FindObjectOfType<Timer>().ShowWinner_ServerRpc();
            }
        }
    }
}