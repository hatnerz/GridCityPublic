using Assets.Scripts.Dtos;
using Assets.Scripts.Multiplayer;
using System;
using UnityEngine;

namespace Assets.Scripts.Networking
{
    public class MultiplayerManager : MonoBehaviour
    {
        [SerializeField] private AuthPanelManager _authPanel;
        [SerializeField] private LobbiesPanelManager _lobbiesPanel;
        [SerializeField] private LobbyPanelManager _lobbyPanel;
        [SerializeField] private CreateLobbyPanelManager _createLobbyPanel;

        private GameManager _gameManager;

        private void Start()
        {
            var token = CredentialsManager.GetAuthToken();
            _gameManager = GameManager.Instance;

            if (string.IsNullOrEmpty(token))
            {
                DisplayAuthForm(true);
                DisplayLobbies(false);
            }
            else
            {
                DisplayAuthForm(false);
                DisplayLobbies(true);
            }

            DisplayCreateLobbyForm(false);
            DisplayLobbyPanel(false);

            _authPanel.LoggedIn += LoggedInHandler;
            _createLobbyPanel.MyLobbyCreated += LobbyJoinedHandler;
            _lobbiesPanel.LobbyCreateButtonClicked += LobbyCreateFormOpenHandler;
            _lobbiesPanel.LobbyJoined += LobbyJoinedHandler;
            _createLobbyPanel.BackToLobbiesButtonClicked += BackToLobbiesHandler;
            _lobbyPanel.GameStarted += StartGameHandler;
        }

        private void OnDestroy()
        {
            _authPanel.LoggedIn -= LoggedInHandler;
            _createLobbyPanel.MyLobbyCreated -= LobbyJoinedHandler;
            _lobbiesPanel.LobbyCreateButtonClicked -= LobbyCreateFormOpenHandler;
        }

        private void LoggedInHandler()
        {
            DisplayAuthForm(false);
            DisplayLobbies(true);
        }

        private void LobbyCreateFormOpenHandler()
        {
            DisplayLobbies(false);
            DisplayCreateLobbyForm(true);
        }

        private void LobbyJoinedHandler(LobbyDetailsDto lobbyDetails)
        {
            DisplayLobbyPanel(true);
            DisplayLobbies(false);
            DisplayCreateLobbyForm(false);

            _lobbyPanel.UpdateLobbyInformation(lobbyDetails);
        }

        private void BackToLobbiesHandler()
        {
            DisplayLobbies(true);
            DisplayCreateLobbyForm(false);
            DisplayLobbyPanel(false);
        }
        
        private void StartGameHandler(Guid lobbyId)
        {
            Debug.Log("Starting multiplayer game from manager");
            _gameManager.StartMultiplayerGame(lobbyId);
        }

        private void DisplayAuthForm(bool isVisible)
        {
            _authPanel.gameObject.SetActive(isVisible);
        }

        private void DisplayLobbies(bool isVisible)
        {
            _lobbiesPanel.gameObject.SetActive(isVisible);
        }

        private void DisplayLobbyPanel(bool isVisible)
        {
            _lobbyPanel.gameObject.SetActive(isVisible);
        }

        private void DisplayCreateLobbyForm(bool isVisible)
        {
            _createLobbyPanel.gameObject.SetActive(isVisible);
        }
    }
}
