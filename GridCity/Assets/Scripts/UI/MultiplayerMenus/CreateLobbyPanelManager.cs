using Assets.Scripts.Dtos;
using Assets.Scripts.Networking;
using System;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Multiplayer
{
    internal class CreateLobbyPanelManager : MonoBehaviour
    {
        [SerializeField] private Button _createLobbyButton;
        [SerializeField] private Button _backToLobbiesButton;
        [SerializeField] private TMP_InputField _lobbyNameInputField;
        [SerializeField] private TMP_InputField _maxPlayersInputField;
        [SerializeField] private TMP_InputField _mapSizeXInputField;
        [SerializeField] private TMP_InputField _mapSizeYInputField;
        [SerializeField] private TMP_InputField _deckSizeInputField;

        private LobbiesSignalRClient _lobbiesSignalRClient;

        public event Action<LobbyDetailsDto> MyLobbyCreated;
        public event Action BackToLobbiesButtonClicked;

        public async void Start()
        {
            _lobbiesSignalRClient = LobbiesSignalRClient.Instance;
            if (!_lobbiesSignalRClient.IsInitialized)
                await _lobbiesSignalRClient.ConnectToHubAsync();

            _lobbiesSignalRClient.MyLobbyCreated += MyLobbyCreatedHandler;
            _backToLobbiesButton.onClick.AddListener(() => BackToLobbiesButtonClicked?.Invoke());
            _createLobbyButton.onClick.AddListener(async () => await CreateLobbyAsync());
        }

        private async Task CreateLobbyAsync()
        {
            string lobbyName = _lobbyNameInputField.text;
            int mapSizeX = int.Parse(_mapSizeXInputField.text);
            int mapSizeY = int.Parse(_mapSizeYInputField.text);
            int deckSize = int.Parse(_deckSizeInputField.text);

            await _lobbiesSignalRClient.CreateLobbyAsync(new CreateLobbyDto()
            {
                LobbyName = lobbyName,
                DeckSize = deckSize,
                MapSizeX = mapSizeX,
                MapSizeY = mapSizeY
            });
        }

        private void MyLobbyCreatedHandler(LobbyDetailsDto lobby)
        {
            MyLobbyCreated?.Invoke(lobby);
        }
    }
}
