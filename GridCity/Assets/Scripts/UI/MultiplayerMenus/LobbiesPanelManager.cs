using Assets.Scripts.Dtos;
using Assets.Scripts.UI.Lobbies;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Networking
{
    internal class LobbiesPanelManager : MonoBehaviour
    {
        [SerializeField] private Transform _lobbiesContent;
        [SerializeField] private GameObject _lobbyItemPrefab;
        [SerializeField] private TMP_Text _authorizedPlayerText;
        [SerializeField] private TMP_Text _totalLobbiesText;
        [SerializeField] private Button _createLobbyButton;

        private LobbiesSignalRClient _lobbiesSignalRClient;

        public event Action LobbyCreateButtonClicked;
        public event Action<LobbyDetailsDto> LobbyJoined;

        public async void OnEnable()
        {
            _lobbiesSignalRClient = LobbiesSignalRClient.Instance;
            _lobbiesSignalRClient.LobbiesUpdated += UpdateLobbies;
            _lobbiesSignalRClient.PlayerJoined += LobbyJoinedHandler;
            if(!_lobbiesSignalRClient.IsInitialized)
                await _lobbiesSignalRClient.ConnectToHubAsync();

            await _lobbiesSignalRClient.GetAllLobbiesAsync();
            UpdateAuthorizedPlayer();

            _createLobbyButton.onClick.AddListener(() => LobbyCreateButtonClicked?.Invoke());
        }

        public void OnDisable()
        {
            if(_lobbiesSignalRClient != null)
            {
                _lobbiesSignalRClient.LobbiesUpdated -= UpdateLobbies;
                _lobbiesSignalRClient.PlayerJoined -= LobbyJoinedHandler;
            }
        }

        private void UpdateLobbies(List<LobbyItemDto> lobbies)
        {
            try
            {
                Debug.Log("Updating lobbies...");
                foreach (Transform child in _lobbiesContent)
                {
                    Debug.Log("Destroing children..");
                    Destroy(child.gameObject);
                }

                Debug.Log("Destroyed childrens");

                foreach (var lobby in lobbies)
                {
                    var item = Instantiate(_lobbyItemPrefab, _lobbiesContent);
                    var lobbyView = item.GetComponent<LobbyItem>();
                    lobbyView.LobbyNameText.text = lobby.Name;
                    lobbyView.LobbyLeaderText.text = lobby.CreatedPlayerName;
                    lobbyView.JoinedPlayersText.text = $"{lobby.JoinedPlayers}/{lobby.MaxPlayers}";
                    if (!lobby.CanJoin)
                    {
                        lobbyView.JoinLobbyButton.interactable = false;
                    }
                    else
                    {
                        lobbyView.JoinLobbyButton.onClick.AddListener(async () => await _lobbiesSignalRClient.JoinLobbyAsync(lobby.Id));
                    }
                }

                _totalLobbiesText.text = $"Total Lobbies: {(lobbies == null ? 0 : lobbies.Count)}";
            }
            catch (Exception e)
            {
                Debug.Log($"Error while updating lobbied: {e.Message}");
            }
        }
    
        private void UpdateAuthorizedPlayer()
        {
            var username = CredentialsManager.GetCurrentUser()?.Username ?? "";
            _authorizedPlayerText.text = $"Logged in as: {username}";
        }

        private void LobbyJoinedHandler(LobbyDetailsDto lobby)
        {
            LobbyJoined?.Invoke(lobby);
        }
    }
}
