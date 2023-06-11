using FirefighterFighter.Networking;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace FirefighterFighter.Game
{
    public class Timer : NetworkBehaviour
    {
        [SerializeField] private TextMeshProUGUI _timerText;
        private float _timer = 120f;

        public void StartTimer()
        {
            if (!IsHost) { return; }
            RefreshTimer_ClientRpc();
        }

        [ClientRpc]
        public void RefreshTimer_ClientRpc()
        {
            StopAllCoroutines();
            StartCoroutine(Routine());

            IEnumerator Routine()
            {
                while (_timer > 0f)
                {
                    _timer--;
                    _timerText.text = _timer.ToString("F0");
                    yield return new WaitForSeconds(1f);
                }

                OnTimeOver();
            }
        }

        private void OnTimeOver()
        {
            List<PlayerPoints> playerPoints = FindObjectsOfType<PlayerPoints>().ToList();
            NetworkObject winner = playerPoints[0].NetworkObject;
            int player = playerPoints[0].TotalPoints;

            for (int i = 0; i < playerPoints.Count; i++)
            {
                if (playerPoints[i].TotalPoints > player)
                {
                    winner = playerPoints[i].NetworkObject;
                }
            }

            string winnerName = winner.GetComponent<UserData>().Player.Data["name"].Value;
            ShowWinner_ClientRpc(winnerName);
        }

        [ServerRpc(RequireOwnership = false)]
        public void ShowWinner_ServerRpc()
        {
            NetworkObject winner = NetworkManager.ConnectedClients[0].PlayerObject;

            for (int i = 0; i < NetworkManager.ConnectedClients.Count; i++)
            {
                if (!NetworkManager.ConnectedClients[(ulong)i].PlayerObject.GetComponent<PlayerHealth>().IsDied)
                {
                    winner = NetworkManager.ConnectedClients[(ulong)i].PlayerObject;
                }
            }

            string winnerName = winner.GetComponent<UserData>().Player.Data["name"].Value;
            ShowWinner_ClientRpc(winnerName);
        }

        [ClientRpc]
        public void ShowWinner_ClientRpc(string winnerName)
        {
            _timerText.fontStyle = FontStyles.UpperCase;
            _timerText.text = $"{winnerName} wins!";
            Time.timeScale = 0f;

            StartCoroutine(Routine());
            IEnumerator Routine()
            {
                yield return new WaitForSecondsRealtime(5f);
                SceneManager.LoadScene(0);
            }
        }
    }
}