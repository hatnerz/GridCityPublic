using Assets.Scripts.Score;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BuildingScoreVisualizer : MonoBehaviour
{
    [SerializeField] private GameObject floatingScorePrefab;
    [SerializeField] private ScoreManager scoreManager;
    private Dictionary<BuildingPlace, int> buildingPlacesPreviousScores;

    public BuildingScoreVisualizer()
    {
    }

    public void InitializeBuildingPlaces(List<BuildingPlace> buildingPlaces)
    {
        buildingPlacesPreviousScores = buildingPlaces.ToDictionary(b => b, b => 0);
    }

    public void VisualizeBuildingPlaceNewScore()
    {
        var scoresToUpdate = new Dictionary<BuildingPlace, int>();
        foreach(var key in buildingPlacesPreviousScores.Keys)
        {
            var newScore = scoreManager.CalculateCurrentBuildingPlaceScore(key);
            var oldScore = buildingPlacesPreviousScores[key];
            if (newScore != oldScore)
            {
                scoresToUpdate[key] = newScore;
                CreateFloatingScoreText(oldScore, newScore, key);
            }
        }

        foreach(var newScoreBuildingPlace in scoresToUpdate.Keys)
        {
            buildingPlacesPreviousScores[newScoreBuildingPlace] = scoresToUpdate[newScoreBuildingPlace];
        }
    }

    private void CreateFloatingScoreText(int oldScore, int newScore, BuildingPlace buildingPlace)
    {
        var floatingTextObject = Instantiate(floatingScorePrefab, buildingPlace.transform);
        var scoreText = oldScore > newScore ? $"-{oldScore - newScore}" : $"+{newScore - oldScore}";
        floatingTextObject.GetComponent<FloatingText>().SetText(scoreText);
        floatingTextObject.GetComponent<MeshRenderer>().sortingOrder = 10;
    }
}