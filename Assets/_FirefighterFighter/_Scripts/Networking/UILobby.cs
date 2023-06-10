using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace FirefighterFighter.Networking
{
    public class UILobby : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _lobbyNameText;
        [SerializeField] private TextMeshProUGUI _joinCodeText;

        public void Set(string lobbyName, string joinCode)
        {
            _lobbyNameText.text = lobbyName;
            _joinCodeText.text = joinCode;
        }
    }
}