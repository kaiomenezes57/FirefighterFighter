using System.Collections.Generic;
using TMPro;
using Unity.Services.Lobbies.Models;
using UnityEngine.UI;
using UnityEngine;

namespace FirefighterFighter.Networking
{
    public class LobbyVisual : MonoBehaviour
    {
        private LobbyManager _manager;

        [SerializeField] private Button _createLobbyButton;
        [SerializeField] private Button _joinLobbyButton;
        [SerializeField] private Button _exitLobbyButton;

        [SerializeField] private Button _startGameButton;

        [SerializeField] private TMP_InputField _playerNameInput;
        [SerializeField] private TMP_InputField _lobbyCodeInput;
        [SerializeField] private List<TextMeshProUGUI> _playerNameText = new();
        [SerializeField] private TextMeshProUGUI _lobbyCodeText;

        [SerializeField] private GameObject _mainLobbyScreen;
        [SerializeField] private GameObject _LobbyRoomScreen;
        [SerializeField] private GameObject _lobbyListScreen;

        private void Start()
        {
            GoToMainScreen();
          
            _manager = FindObjectOfType<LobbyManager>();
            _createLobbyButton.onClick.AddListener(_manager.OnCreateLobby);
            _joinLobbyButton.onClick.AddListener(() => _manager.OnJoinLobby(_lobbyCodeInput.text));
            _startGameButton.onClick.AddListener(_manager.OnStartGame);
        }

        public void ShowPlayersInLobbyScreen(Lobby clientLobby)
        {
            for (int i = 0; i < clientLobby.Players.Count; i++)
            {
                _playerNameText[i].text = clientLobby.Players[i].Data["name"].Value;
            }
        }

        public void GoToMainScreen()
        {
            _mainLobbyScreen.SetActive(true);
            _LobbyRoomScreen.SetActive(false);
            _lobbyListScreen.SetActive(false);
            _startGameButton.gameObject.SetActive(false);

            _lobbyCodeText.text = null;
        }

        public void GoToLobbyRoom(bool isHost, string lobbyCode)
        {
            _mainLobbyScreen.SetActive(false);
            _LobbyRoomScreen.SetActive(true);
            _lobbyListScreen.SetActive(false);
            _startGameButton.gameObject.SetActive(isHost);
            
            _lobbyCodeText.text = lobbyCode;
        }

        public void GoToLobbyList(List<Lobby> lobbies)
        {
            _mainLobbyScreen.SetActive(false);
            _LobbyRoomScreen.SetActive(false);
            _lobbyListScreen.SetActive(true);
            _startGameButton.gameObject.SetActive(false);

            _lobbyCodeText.text = null;
            SpawnLobbiesOnScreen(lobbies);
        }

        private void SpawnLobbiesOnScreen(List<Lobby> lobbies)
        {
            if (lobbies == null || lobbies.Count == 0) { return; }
            GameObject resource = Resources.Load("Lobby") as GameObject;
            Transform scrollView = GetComponentInChildren<ScrollRect>().transform;

            for (int i = 0; i < lobbies.Count; i++)
            {
                UILobby lobby = Instantiate(resource, scrollView).GetComponent<UILobby>();
                lobby.Set(lobbies[i].Name, lobbies[i].LobbyCode);
            }
        }

        public void StartGame()
        {
            _LobbyRoomScreen.transform.parent.gameObject.SetActive(false);
        }

        public string GetPlayerNameInput()
        {
            return _playerNameInput.text;
        }
    }
}