using Unity.Netcode;

namespace FirefighterFighter.Game
{
    public class PlayerPoints : NetworkBehaviour
    {
        public int TotalPoints { get { return _totalPoints; } set { _totalPoints = value; } }
        private int _totalPoints;

        public override void OnNetworkSpawn()
        {
            if (!IsOwner) { return; }
            base.OnNetworkSpawn();
        }

        [ServerRpc(RequireOwnership = false)]
        public void AddPoints_ServerRpc(int amount, ServerRpcParams rpcParams = default)
        {
            ulong clientId = rpcParams.Receive.SenderClientId;
            
            for (int i = 0; i < NetworkManager.Singleton.ConnectedClients.Count; i++)
            {
                if (NetworkManager.Singleton.ConnectedClients[(ulong)i].ClientId == clientId)
                {
                    NetworkManager.Singleton.ConnectedClients[(ulong)i].PlayerObject.
                        GetComponent<PlayerPoints>().TotalPoints += amount;
                }
            }

            FindObjectOfType<PlayerStats>().UpdateVisual_ServerRpc();
        }
    }
}