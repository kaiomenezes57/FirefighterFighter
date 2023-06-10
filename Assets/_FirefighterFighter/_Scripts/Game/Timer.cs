using FirefighterFighter.Networking;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Netcode;
using UnityEngine;

namespace FirefighterFighter.Game
{
    public class Timer : NetworkBehaviour
    {
        [SerializeField] private TextMeshProUGUI _timerText;
        private float _timer = 120f;

        private void Start()
        {
            _timerText.text = null;
        }

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

        [ClientRpc]
        private void ShowWinner_ClientRpc(string winnerName)
        {
            _timerText.fontStyle = FontStyles.UpperCase;
            _timerText.text = $"{winnerName} wins!";
            Time.timeScale = 0f;
        }
    }
}