using Assets.Scripts.Dtos;
using Microsoft.AspNetCore.SignalR.Client;
using PimDeWitte.UnityMainThreadDispatcher;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using static System.Net.WebRequestMethods;

namespace Assets.Scripts.Networking
{
    internal class GameplaySignalRClient : IDisposable
    {
        private static GameplaySignalRClient _instance = new();
        public static GameplaySignalRClient Instance => _instance;


        private const string _gameplayHubUrl = "https://localhost:7259/gameplay";
        //private const string _gameplayHubUrl = "https://gridcityserver-app-2025040316422.icyhill-0198d417.polandcentral.azurecontainerapps.io";
        private HubConnection _connection;
        private bool _isConnecting = false;

        public bool IsInitialized => _connection != null && _connection.State != HubConnectionState.Disconnected;

        public event Action<GameSessionInitialDto> GameStartConfirmed;
        public event Action<MoveResultDto> MoveMade;
        public event Action<PlayerCardsDto> ActualCardsUpdated;
        public event Action<List<PlayerScoreDto>> ScoresUpdated;
        public event Action<GameSessionFinalResultDto> GameEnded;

        private GameplaySignalRClient()
        {
        }

        public async Task ConnectToHubAsync()
        {
            if(_isConnecting)
                return;

            _isConnecting = true;
            var authToken = CredentialsManager.GetAuthToken();
            if (string.IsNullOrEmpty(authToken))
            {
                Debug.Log("Auth token is empty");
                return;
            }

            _connection = new HubConnectionBuilder()
                .WithUrl(_gameplayHubUrl, options =>
                {
                    options.AccessTokenProvider = () => Task.FromResult(authToken);
                })
                .Build();

            _connection.On<GameSessionInitialDto>(EventsNames.GameStartConfirmed, initialDto =>
            {
                UnityMainThreadDispatcher.Instance().Enqueue(() =>
                {
                    GameStartConfirmed?.Invoke(initialDto);
                });
            });

            _connection.On<MoveResultDto>(EventsNames.MoveMade, moveDto =>
            {
                UnityMainThreadDispatcher.Instance().Enqueue(() =>
                {
                    MoveMade?.Invoke(moveDto);
                });
            });

            _connection.On<PlayerCardsDto>(EventsNames.ActualCardsUpdated, cardsDto =>
            {
                UnityMainThreadDispatcher.Instance().Enqueue(() =>
                {
                    ActualCardsUpdated?.Invoke(cardsDto);
                });
            });

            _connection.On<List<PlayerScoreDto>>(EventsNames.ScoreUpdated, scoreDto =>
            {
                UnityMainThreadDispatcher.Instance().Enqueue(() =>
                {
                    Debug.Log("receivedScore");
                    ScoresUpdated?.Invoke(scoreDto);
                });
            });

            _connection.On<GameSessionFinalResultDto>(EventsNames.GameEnded, gameEndDto =>
            {
                UnityMainThreadDispatcher.Instance().Enqueue(() =>
                {
                    GameEnded?.Invoke(gameEndDto);
                    _connection.StopAsync().GetAwaiter().GetResult();
                    _connection = null;
                });
            });

            try
            {
                await _connection.StartAsync();
                Debug.Log("Joined to gameplay hub");
                _isConnecting = false;
            }
            catch (Exception ex)
            {
                Debug.LogError("Error joining gameplay hub: " + ex.Message);
            }
        }

        public async Task MakeMoveAsync(GameSessionMoveDto moveDto)
        {
            VerifyHubConnection();
            await _connection.SendAsync(ActionMethodNames.MakeMove, moveDto);
        }

        private void VerifyHubConnection()
        {
            if (_connection == null || _connection.State != HubConnectionState.Connected)
                throw new InvalidOperationException("Not connected to the gameplay hub");
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
