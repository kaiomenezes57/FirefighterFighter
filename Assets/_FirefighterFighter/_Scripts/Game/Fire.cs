using Unity.Netcode;
using UnityEngine;

namespace FirefighterFighter.Game
{
    public class Fire : NetworkBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                FireExtinguish_ServerRpc();
                GetComponent<Collider>().enabled = false;
            }
        }

        [ServerRpc(RequireOwnership = false)]
        private void FireExtinguish_ServerRpc(ServerRpcParams serverRpcParams = default)
        {
            ulong clientID = serverRpcParams.Receive.SenderClientId;
            FireExtinguish_ClientRpc(clientID);
            NetworkObject.Despawn();
        }

        [ClientRpc]
        private void FireExtinguish_ClientRpc(ulong clientID)
        {
            if (NetworkManager.LocalClientId == clientID)
            {
                NetworkManager.LocalClient.PlayerObject.GetComponent<PlayerPoints>().AddPoints_ServerRpc(Random.Range(10, 15));
            }
         
            Destroy(gameObject);
        }
    }
}