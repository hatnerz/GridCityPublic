using Assets.Scripts.Core;
using Assets.Scripts.Dtos;
using Assets.Scripts.Networking;
using Assets.Scripts.UI.Hud;
using Assets.Scripts.UI.OverlayMenus;
using GridCity.GameLogic.CellElements.BaseElements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Scripts.Multiplayer.Gameplay
{
    public class MultiplayerGameplayManager : MonoBehaviour
    {
        [SerializeField] private MultiplayerCardManager _cardManager;
        [SerializeField] private GridManager _gridManager;
        [SerializeField] private MultiplayerGameInformation _multiplayerGameInformation;
        [SerializeField] private BuildingScoreVisualizer _buildingScoreVisualizer;
        [SerializeField] private GameObject _endGameMenuPrefab;
        [SerializeField] private Canvas _gameHudCanvas;

        private GameplaySignalRClient _gameplaySignalRClient;
        private List<PlayerDetailsDto> _players = new List<PlayerDetailsDto>();

        public async void Start()
        {
            _gameplaySignalRClient = GameplaySignalRClient.Instance;
            _gameplaySignalRClient.GameStartConfirmed += GameStartConfirmedHandler;
            _gameplaySignalRClient.MoveMade += MoveMadeHandler;
            _gameplaySignalRClient.ActualCardsUpdated += ActualCardsUpdatedHandler;
            _gameplaySignalRClient.ScoresUpdated += ScoreUpdatedHandler;
            _gameplaySignalRClient.GameEnded += GameEndHandler;

            _gridManager.ValidBuildingPlaceClicked += ValidBuildingPlaceClickedHandler;

            Debug.Log("!! MULTIPLAYER initializing");
            if (!_gameplaySignalRClient.IsInitialized)
            {
                Debug.Log("connecting to hub");
                await _gameplaySignalRClient.ConnectToHubAsync();
                Debug.Log("connected to hub");
            }
        }

        /*private void OnDestroy()
        {
            Debug.Log("!! MULTIPLAYER disconnection");

            if (_gameplaySignalRClient!= null)
            {
                _gameplaySignalRClient.GameStartConfirmed -= GameStartConfirmedHandler;
                _gameplaySignalRClient.MoveMade -= MoveMadeHandler;
                _gameplaySignalRClient.ActualCardsUpdated -= ActualCardsUpdatedHandler;
                _gameplaySignalRClient.ScoresUpdated -= ScoreUpdatedHandler;
                _gameplaySignalRClient.GameEnded -= GameEndHandler;
            }

            if(_gridManager != null)
            {
                _gridManager.ValidBuildingPlaceClicked -= ValidBuildingPlaceClickedHandler;
           }
        }*/

        private async void ValidBuildingPlaceClickedHandler(BuildingPlace buildingPlace)
        {
            var buildingCard = _cardManager.TryPlayActiveCard();

            if (buildingCard == null)
            {
                Debug.Log("Building card not picked");
                return;
            }

            var moveDto = new GameSessionMoveDto()
            {
                GridPositionX = buildingPlace.GridPosition.X,
                GridPositionY = buildingPlace.GridPosition.Y,
                PlayedCard = buildingCard.Type
            };

            await _gameplaySignalRClient.MakeMoveAsync(moveDto);
        }

        private void GameStartConfirmedHandler(GameSessionInitialDto gameStartDto)
        {
            Debug.Log("Game start confirmed");
            _gridManager.InitializeGrid(new Vector2Int(gameStartDto.GridSizeX, gameStartDto.GridSizeY), true);
            _buildingScoreVisualizer.InitializeBuildingPlaces(
                _gridManager.BuildingPlaces.Cast<BuildingPlace>().ToList());

            var currentPlayerId = CredentialsManager.GetCurrentUser()?.Id ?? Guid.Empty;
            GameState.IsPlayersTurn = (currentPlayerId == gameStartDto.FirstTurnPlayer);

            _players = gameStartDto.Players
                .Select(player => new PlayerDetailsDto() { Id = player.Id, Score = 0, Name = player.Username, Color = ParsePlayerColorOrDefault(player.Color)})
                .ToList();

            _multiplayerGameInformation.Initialize(
                GameState.IsPlayersTurn,
                gameStartDto.Players.FirstOrDefault(e => e.Id == gameStartDto.FirstTurnPlayer)?.Username ?? "",
                gameStartDto.InitialDeckSize,
                0,
                0,
                _players
               );

            _multiplayerGameInformation.DisableWaitingForPlayersText();
        }

        private void MoveMadeHandler(MoveResultDto moveResultDto)
        {
            var currentPlayerId = CredentialsManager.GetCurrentUser()?.Id ?? Guid.Empty;
            GameState.IsPlayersTurn = currentPlayerId == moveResultDto.NextTurnPlayerId;

            var building = BuildingFactory.CreateBuilding(moveResultDto.PlacedBuilding);
            var playerColor = _players.FirstOrDefault(e => e.Id == moveResultDto.PlacedPlayerId)?.Color;
            _gridManager.PlaceBuilding(building, new Vector2Int(moveResultDto.GridPositionX, moveResultDto.GridPositionY), playerColor);
            _buildingScoreVisualizer.VisualizeBuildingPlaceNewScore();
        }

        private void ActualCardsUpdatedHandler(PlayerCardsDto playerCardsDto)
        {
            _multiplayerGameInformation.UpdateRemainsCardsInformation(playerCardsDto.CardsLeftInDeck);
            _cardManager.UpdatePlayerActualCards(playerCardsDto.CardsInHand);
        }

        private void ScoreUpdatedHandler(List<PlayerScoreDto> playerScoresList)
        {
            Debug.Log($"Received scores: {JsonSerializer.Serialize(playerScoresList)}");
            _players.ForEach(p =>
            {
                var playerScore = playerScoresList.FirstOrDefault(ps => ps.PlayerId == p.Id)?.Score;
                Debug.Log(JsonSerializer.Serialize(playerScore));
                if (playerScore != null)
                    p.Score = playerScore.Value;
            });

            //Debug.Log($"Players: {_players[0].Id} {_players[0].Score} {_players[0].Name}");

            var currentUserId = CredentialsManager.GetCurrentUser()?.Id ?? Guid.Empty;
            var yourScore = _players.FirstOrDefault(p => p.Id == currentUserId)?.Score ?? 0;
            _multiplayerGameInformation.UpdateScoreInformation(yourScore, _players);
        }

        private void GameEndHandler(GameSessionFinalResultDto gameEndDto)
        {
            Debug.Log("Game ended");
            var winner = _players.FirstOrDefault(p => p.Id == gameEndDto.Winner.PlayerId)?.Name ?? "";
            var scores = gameEndDto.PlayerScores
                .OrderByDescending(e => e.Score)
                .Select((e, i) => new PlayerScoreDisplayDto()
                {
                    Name = _players.FirstOrDefault(p => p.Id == e.PlayerId)?.Name ?? "",
                    Score = e.Score,
                    Position = i + 1
                })
                .ToList(); 

            var endGameMenu = Instantiate(_endGameMenuPrefab, _gameHudCanvas.transform);
            endGameMenu.GetComponent<EndGameMenuManager>().SetFinalScoresInformation(winner, scores);
            gameObject.SetActive(false);
        }

        private Color ParsePlayerColorOrDefault(string hexColor)
        {
            Color color;

            if (!ColorUtility.TryParseHtmlString(hexColor, out color))
            {
                color = Color.red;
            }

            return color;
        }
    }
}
