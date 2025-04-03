using Assets.Scripts.Core;
using Assets.Scripts.Dtos;
using Assets.Scripts.Networking;
using System;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
namespace Assets.Scripts.Multiplayer
{
    internal class LobbyPanelManager : MonoBehaviour
    {
        [SerializeField] private Transform _connectedPlayersContent;
        [SerializeField] private Button _startGameButton;
        [SerializeField] private TMP_Text _connectedPlayersText;
        [SerializeField] private TMP_Text _lobbyNameText;
        [SerializeField] private TMP_Text _lobbyLeaderText;
        [SerializeField] private TMP_Text _mapSizeText;
        [SerializeField] private TMP_Text _deckSizeText;
        [SerializeField] private TMP_Text _gameStartedText;
        [SerializeField] private GameObject _playerTextPrefab;

        private LobbiesSignalRClient _lobbiesSignalRClient;
        private Guid _lobbyId = Guid.Empty;

        public event Action<Guid> GameStarted;

        private async void OnEnable()
        {
            _gameStartedText.enabled = false;
            _startGameButton.gameObject.SetActive(false);
            _lobbiesSignalRClient = LobbiesSignalRClient.Instance;
            _lobbiesSignalRClient.PlayerJoined += UpdateLobbyInformation;
            _lobbiesSignalRClient.PlayerLeft += UpdateLobbyInformation;
            _lobbiesSignalRClient.LobbyRemoved += LobbyRemovedHandler;
            _lobbiesSignalRClient.GameStarted += GameStartedHandler;
            _startGameButton.onClick.AddListener(async () => await StartGameAsync());

            if (!_lobbiesSignalRClient.IsInitialized)
                await _lobbiesSignalRClient.ConnectToHubAsync();
        }

        private void OnDisable()
        {
            _lobbiesSignalRClient.PlayerJoined -= UpdateLobbyInformation;
            _lobbiesSignalRClient.PlayerLeft -= UpdateLobbyInformation;
            _lobbiesSignalRClient.LobbyRemoved -= LobbyRemovedHandler;
        }

        public void UpdateLobbyInformation(LobbyDetailsDto lobbyData)
        {
            _lobbyId = lobbyData.Id;
            _lobbyNameText.text = $"Name: {lobbyData.Name}";
            _lobbyLeaderText.text = $"Leader: {lobbyData.CreatedPlayerName}";
            _mapSizeText.text = $"Map Size: {lobbyData.MapSizeX}x{lobbyData.MapSizeY}";
            _deckSizeText.text = $"Deck Size: {lobbyData.DeckPerPlayerSize}";
            _connectedPlayersText.text = $"Connected Players: {lobbyData.JoinedPlayers}/{lobbyData.MaxPlayers}";
            foreach (Transform child in _connectedPlayersContent)
            {
                Destroy(child.gameObject);
            }

            foreach (var player in lobbyData.Players)
            {
                GameObject playerTextObject = Instantiate(_playerTextPrefab, _connectedPlayersContent);
                TMP_Text playerText = playerTextObject.GetComponent<TMP_Text>();
                playerText.text = player;
            }

            if(CredentialsManager.GetCurrentUser().GetValueOrDefault().Id == lobbyData.CreatedPlayerId)
                _startGameButton.gameObject.SetActive(true);
        }

        private async Task StartGameAsync()
        {
            await _lobbiesSignalRClient.StartGameAsync(_lobbyId);
        }

        private void GameStartedHandler(LobbyDetailsDto lobbyDetails)
        {
            if(_gameStartedText != null)
                _gameStartedText.enabled = true;
            GameStarted?.Invoke(lobbyDetails.Id);
        }

        private void LobbyRemovedHandler(LobbyDetailsDto lobbyData)
        {
            // TODO: Add lobby removal handling logic
        }
    }
}
