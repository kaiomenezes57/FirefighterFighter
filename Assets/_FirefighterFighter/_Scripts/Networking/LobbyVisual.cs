using System.Collections.Generic;
using TMPro;
using Unity.Services.Lobbies.Models;
using UnityEngine.UI;
using UnityEngine;
using Unity.Netcode;

namespace FirefighterFighter.Networking
{
    public class LobbyVisual : MonoBehaviour
    {
        private LobbyManager _manager;
        
        [Header("Buttons")]
        [SerializeField] private Button _createLobbyButton;
        [SerializeField] private Button _joinLobbyButton;
        [SerializeField] private Button _exitLobbyButton;
        [SerializeField] private Button _startGameButton;

        [Space][Header("Texts")]
        [SerializeField] private TMP_InputField _playerNameInput;
        [SerializeField] private TMP_InputField _lobbyCodeInput;
        [SerializeField] private TextMeshProUGUI _lobbyCodeText;
        [SerializeField] private List<TextMeshProUGUI> _playerNameText = new();

        [Space][Header("Panels")]
        [SerializeField] private GameObject _lobbyRoomScreen;
        [SerializeField] private GameObject _mainScreen;

        private void Start()
        {
            if (PlayerPrefs.HasKey("nickname"))
            {
                _playerNameInput.SetTextWithoutNotify(PlayerPrefs.GetString("nickname"));
            }

            _manager = FindObjectOfType<LobbyManager>();
            _createLobbyButton.onClick.AddListener(() => _manager.OnCreateLobby());
            _joinLobbyButton.onClick.AddListener(() => _manager.OnJoinLobby(_lobbyCodeInput.text));
            _startGameButton.onClick.AddListener(() => _manager.OnStartGame());
            _exitLobbyButton.onClick.AddListener(() => _manager.OnLeaveLobby());

            _startGameButton.interactable = false;
            _joinLobbyButton.interactable = false;
            _lobbyCodeInput.onValueChanged.AddListener((str) => {
                _joinLobbyButton.interactable = !string.IsNullOrEmpty(str);
            });
        }

        public void ShowPlayersInLobbyScreen(Lobby clientLobby)
        {
            for (int i = 0; i < clientLobby.Players.Count; i++)
            {
                _playerNameText[i].text = clientLobby.Players[i].Data["name"].Value;
            }

#if !UNITY_EDITOR
            _startGameButton.interactable = clientLobby.Players.Count > 1;
#else
            _startGameButton.interactable = true;
#endif
        }

        public void GoToMainScreen()
        {
            _lobbyCodeText.text = null;
            PlayerPrefs.SetString("nickname", _playerNameInput.text);

            _mainScreen.SetActive(true);
        }

        public void GoToLobbyRoom(bool isHost, string lobbyCode)
        {
            _startGameButton.gameObject.SetActive(isHost);
            _lobbyCodeText.text = lobbyCode;

            _lobbyRoomScreen.SetActive(true);
        }

        public void GoToLobbyList(List<Lobby> lobbies)
        {
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
            _lobbyRoomScreen.transform.parent.gameObject.SetActive(false);
        }

        public string GetPlayerNameInput()
        {
            return _playerNameInput.text;
        }
    }
}