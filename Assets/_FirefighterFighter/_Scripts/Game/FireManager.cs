using FirefighterFighter.Networking;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

namespace FirefighterFighter.Game
{
    public class FireManager : NetworkBehaviour
    {
        private readonly List<FireSpawner> _fireSpawners = new();

        public void StartMode()
        {
            if (!IsHost) { return; }

            _fireSpawners.AddRange(FindObjectsOfType<FireSpawner>());
            StartCoroutine(SpawnRoutine());
        }

        private IEnumerator SpawnRoutine()
        {
            InitialServerSetup server = FindObjectOfType<InitialServerSetup>();
            GameObject firePrefab = Resources.Load("Fire") as GameObject;

            while (true)
            {
                yield return new WaitForSeconds(Random.Range(10f, 20f));
                Vector3 position = _fireSpawners[Random.Range(0, _fireSpawners.Count)].transform.position;

                GameObject fire = Instantiate(firePrefab);
                fire.transform.position = position;
                fire.GetComponent<NetworkObject>().Spawn();
            }
        }
    }
}