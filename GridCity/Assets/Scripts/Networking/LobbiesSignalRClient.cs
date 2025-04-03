using Assets.Scripts.Dtos;
using Microsoft.AspNetCore.SignalR.Client;
using PimDeWitte.UnityMainThreadDispatcher;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Networking
{
    internal class LobbiesSignalRClient : IDisposable
    {
        private static LobbiesSignalRClient _instance = new();
        public static LobbiesSignalRClient Instance => _instance;

        private const string _lobbyHubUrl = "https://localhost:7259/lobbies";
        //private const string _lobbyHubUrl = "https://gridcityserver-app-2025040316422.icyhill-0198d417.polandcentral.azurecontainerapps.io";
        private HubConnection _connection;

        public bool IsInitialized =>  _connection != null && _connection.State != HubConnectionState.Disconnected;
        public event Action<List<LobbyItemDto>> LobbiesUpdated;
        public event Action<LobbyDetailsDto> MyLobbyCreated;
        public event Action<LobbyDetailsDto> LobbyRemoved;
        public event Action<LobbyDetailsDto> PlayerJoined;
        public event Action<LobbyDetailsDto> PlayerLeft;
        public event Action<LobbyDetailsDto> GameStarted;

        private LobbiesSignalRClient()
        {
        }

        public async Task ConnectToHubAsync()
        {
            var authToken = CredentialsManager.GetAuthToken();
            if (string.IsNullOrEmpty(authToken))
            {
                Debug.Log("Auth token is empty");
                return;
            }

            _connection = new HubConnectionBuilder()
                .WithUrl(_lobbyHubUrl, options =>
                {
                    options.AccessTokenProvider = () => Task.FromResult(authToken);
                })
                .Build();

            _connection.On<List<LobbyItemDto>>(EventsNames.LobbiesUpdated, lobbies =>
            {
                UnityMainThreadDispatcher.Instance().Enqueue(() =>
                {
                    LobbiesUpdated?.Invoke(lobbies);
                });
            });

            _connection.On<LobbyDetailsDto>(EventsNames.MyLobbyCreated, lobby =>
            {
                UnityMainThreadDispatcher.Instance().Enqueue(() =>
                {
                    MyLobbyCreated?.Invoke(lobby);
                });
            });

            _connection.On<LobbyDetailsDto>(EventsNames.LobbyRemoved, lobby =>
            {
                UnityMainThreadDispatcher.Instance().Enqueue(() =>
                {
                    LobbyRemoved?.Invoke(lobby);
                });
            });

            _connection.On<LobbyDetailsDto>(EventsNames.PlayerJoined, lobby =>
            {
                UnityMainThreadDispatcher.Instance().Enqueue(() =>
                {
                    PlayerJoined?.Invoke(lobby);
                });
            });

            _connection.On<LobbyDetailsDto>(EventsNames.PlayerLeft, lobby =>
            {
                UnityMainThreadDispatcher.Instance().Enqueue(() =>
                {
                    PlayerLeft?.Invoke(lobby);
                });
            });

            _connection.On<LobbyDetailsDto>(EventsNames.GameStarted, lobby =>
            {
                UnityMainThreadDispatcher.Instance().Enqueue(() =>
                {
                    GameStarted?.Invoke(lobby);
                });
            });

            try
            {
                await _connection.StartAsync();
                Debug.Log("Joined to lobbies hub");
            }
            catch (Exception ex)
            {
                Debug.LogError("Error joining lobby hub: " + ex.Message);
            }
        }

        public async Task CreateLobbyAsync(CreateLobbyDto createLobbyDto)
        {
            VerifyHubConnection();
            await _connection.SendAsync(ActionMethodNames.CreateLobby, createLobbyDto);
        }

        public async Task GetAllLobbiesAsync()
        {
            VerifyHubConnection();
            Debug.Log("Getting lobbies");
            await _connection.SendAsync(ActionMethodNames.GetLobbies);
        }

        public async Task RemoveLobbyAsync(Guid lobbyId)
        {
            VerifyHubConnection();
            await _connection.SendAsync(ActionMethodNames.RemoveLobby, lobbyId);
        }

        public async Task JoinLobbyAsync(Guid lobbyId)
        {
            VerifyHubConnection();
            await _connection.SendAsync(ActionMethodNames.JoinLobby, lobbyId);
        }

        public async Task LeaveLobbyAsync(Guid lobbyId)
        {
            VerifyHubConnection();
            await _connection.SendAsync(ActionMethodNames.LeaveLobby, lobbyId);
        }

        public async Task StartGameAsync(Guid lobbyId)
        {
            VerifyHubConnection();
            await _connection.SendAsync(ActionMethodNames.StartGame, lobbyId);
        }

        private void VerifyHubConnection()
        {
            if(_connection == null || _connection.State != HubConnectionState.Connected)
                throw new InvalidOperationException("Not connected to the lobbies hub");
        }

        public void Dispose()
        {
            if (_connection != null)
            {
                _connection.StopAsync().GetAwaiter().GetResult();
                _connection.DisposeAsync().AsTask().GetAwaiter().GetResult();
                _connection = null;
            }
        }
    }
}
