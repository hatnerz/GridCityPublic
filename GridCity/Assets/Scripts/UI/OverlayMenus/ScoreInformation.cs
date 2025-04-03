using TMPro;
using UnityEngine;

namespace Assets.Scripts.UI.Hud
{
    public class ScoreInformation : MonoBehaviour
    {
        [SerializeField] private TMP_Text TotalScoreText;
        [SerializeField] private TMP_Text RequiredScoreText;

        public void SetScoreInformation(int totalScore, int requiredScore)
        {
            TotalScoreText.text = $"Total score: {totalScore}";
            RequiredScoreText.text = $"Required score: {requiredScore}";
        }
    }
}
