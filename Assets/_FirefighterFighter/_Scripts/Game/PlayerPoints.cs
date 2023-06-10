using System;
using Unity.Netcode;
using UnityEngine;

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

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                AddPoints_ServerRpc(UnityEngine.Random.Range(1, 200));
            }
        }

        [ServerRpc(RequireOwnership = false)]
        public void AddPoints_ServerRpc(int amount, ServerRpcParams rpcParams = default)
        {
            ulong clientId = rpcParams.Receive.SenderClientId;
            FindObjectOfType<PlayerStats>().UpdatePlayerPoints_ClientRpc(amount, clientId);
        }
    }
}