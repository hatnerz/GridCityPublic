using Assets.Scripts.Core;
using Assets.Scripts.Score;
using Assets.Scripts.UI.Hud;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Gameplay
{
    public class LevelGameplayManager : MonoBehaviour
    {
        [SerializeField] private CardManager cardManager;
        [SerializeField] private GridManager gridManager;
        [SerializeField] private ScoreManager scoreManager;
        [SerializeField] private LevelStateInformation levelStateInformation;
        [SerializeField] private BuildingScoreVisualizer buildingScoreVisualizer;
        [SerializeField] private Canvas levelHudCanvas;
        [SerializeField] private GameObject winMenuPrefab;
        [SerializeField] private GameObject gameOverMenuPrefab;

        private void Start()
        {
            var currentLevel = GameState.CurrentLevelNumber;
            var levelData = ResourceManager.Instance.LevelDataDictionary[currentLevel];
            if (cardManager == null || gridManager == null)
            {
                throw new MissingReferenceException("CardManager or GridManager is not set in LevelInitializer");
            }

            cardManager.InitializeLevelDeck(levelData);
            gridManager.InitializeGrid(levelData.GridSize, false);

            buildingScoreVisualizer.InitializeBuildingPlaces(
                gridManager.BuildingPlaces.Cast<BuildingPlace>().ToList());

            SetInitialLevelHudInformation(levelData);

            cardManager.CardTakenFromDeck += (_) => UpdateDeckHudInformation();
            gridManager.BuildingPlaced += (_) => HandleScore();
            gridManager.ValidBuildingPlaceClicked += ValidBuildingPlaceClickedHandler;
        }

        private void ValidBuildingPlaceClickedHandler(BuildingPlace buildingPlace)
        {
            var buildingCard = cardManager.TryPlayActiveCard();

            if (buildingCard == null)
            {
                Debug.Log("Building card not picked");
                return;
            }

            gridManager.PlaceBuilding(buildingCard.Building, new Vector2Int(buildingPlace.GridPosition.X, buildingPlace.GridPosition.Y));
        }

        private void SetInitialLevelHudInformation(LevelData levelData)
        {
            levelStateInformation.Initialize(
                levelData.LevelNumber,
                levelData.TargetScore,
                scoreManager.CalculateTotalScore(),
                cardManager.InitialDeckSize,
                cardManager.RemainsCardsInDeck
            );
        }

        private void UpdateDeckHudInformation()
        {
            levelStateInformation.UpdateRemainsCardsInformation(cardManager.RemainsCardsInDeck);
        }

        private void HandleScore()
        {
            var currentScore = scoreManager.CalculateTotalScore();
            buildingScoreVisualizer.VisualizeBuildingPlaceNewScore();
            UpdateCurrentScoreHudInformation(currentScore);

            if(cardManager.RemainsCardsInHand == 0)
            {
                EndLevel();
            }
        }

        private void UpdateCurrentScoreHudInformation(int score)
        {
            levelStateInformation.UpdateCurrentScoreInformation(score);
        }

        private void EndLevel()
        {
            var totalPlayerScore = scoreManager.CalculateTotalScore();
            var currentLevelData = ResourceManager.Instance.LevelDataDictionary[GameState.CurrentLevelNumber];
            var requiredScore = currentLevelData.TargetScore;

            Debug.Log(totalPlayerScore);
            Debug.Log(requiredScore);

            if (totalPlayerScore >= requiredScore)
            {
                var winMenu = Instantiate(winMenuPrefab, levelHudCanvas.transform);
                winMenu.GetComponent<WinMenuManager>().SetScoreInformation(totalPlayerScore, requiredScore);
            }
            else
            {
                var loseMenu = Instantiate(gameOverMenuPrefab, levelHudCanvas.transform);
                loseMenu.GetComponent<GameOverMenuManager>().SetScoreInformation(totalPlayerScore, requiredScore);
            }
        }
    }

}
