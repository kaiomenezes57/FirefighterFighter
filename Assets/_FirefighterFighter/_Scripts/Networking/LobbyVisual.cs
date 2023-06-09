using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

namespace FirefighterFighter.Networking
{
    public class LobbyVisual : MonoBehaviour
    {
        private LobbyManager _manager;

        [SerializeField] private Button _createLobbyButton;
        [SerializeField] private Button _joinLobbyButton;
        [SerializeField] private Button _startGameButton;

        [SerializeField] private TMP_InputField _playerNameInput;
        [SerializeField] private TMP_InputField _lobbyCodeInput;
        [SerializeField] private List<TextMeshProUGUI> _playerNameText = new();
        [SerializeField] private TextMeshProUGUI _lobbyCodeText;

        [SerializeField] private GameObject _mainLobbyScreen;
        [SerializeField] private GameObject _LobbyRoomScreen;

        private void Start()
        {
            _manager = FindObjectOfType<LobbyManager>();

            _lobbyCodeText.text = null;

            _startGameButton.gameObject.SetActive(false);
            _mainLobbyScreen.SetActive(true);
            _LobbyRoomScreen.SetActive(false);

            _createLobbyButton.onClick.AddListener(_manager.CreateLobby);
            _joinLobbyButton.onClick.AddListener(_manager.JoinLobbyByCode);
            _startGameButton.onClick.AddListener(_manager.StartGame);
        }

        public void ShowPlayersInLobbyScreen(Lobby clientLobby)
        {
            for (int i = 0; i < clientLobby.Players.Count; i++)
            {
                _playerNameText[i].text = clientLobby.Players[i].Data["name"].Value;
            }
        }

        public void GoToLobbyRoom(bool isHost, string lobbyCode)
        {
            _mainLobbyScreen.SetActive(false);
            _LobbyRoomScreen.SetActive(true);
            _startGameButton.gameObject.SetActive(isHost);
            
            _lobbyCodeText.text = lobbyCode;
        }

        public void StartGame()
        {
            _LobbyRoomScreen.transform.parent.gameObject.SetActive(false);
        }

        public string GetPlayerNameInput()
        {
            return _playerNameInput.text;
        }

        public string GetLobbyCodeInput()
        {
            return _lobbyCodeInput.text;
        }
    }
}