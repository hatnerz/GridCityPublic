using Assets.Scripts.Score;
using TMPro;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

namespace Assets.Scripts.UI.Hud
{
    public class LevelStateInformation : MonoBehaviour
    {
        [SerializeField] private TMP_Text _levelNumberText;
        [SerializeField] private TMP_Text _totalCardsInDeckText;
        [SerializeField] private TMP_Text _remainsCardsInDeckText;
        [SerializeField] private TMP_Text _levelTargetScoreText;
        [SerializeField] private TMP_Text _currentScoreText;

        public void Initialize(
            int levelNumber,
            int targetScore,
            int totalScore,
            int initialDeckSize,
            int remainsCardsInDeck)
        {
            _levelNumberText.text = $"Level {levelNumber}";
            _levelTargetScoreText.text = $"Target score: {targetScore}";
            _currentScoreText.text = $"Current score: {totalScore}";
            _totalCardsInDeckText.text = $"Total cards: {initialDeckSize}";
            _remainsCardsInDeckText.text = $"Remains cards: {remainsCardsInDeck}";
        }

        public void UpdateRemainsCardsInformation(int remainsCards)
        {
            _remainsCardsInDeckText.text = $"Remains cards: {remainsCards}";
        }

        public void UpdateCurrentScoreInformation(int currentScore)
        {
            _currentScoreText.text = $"Current score: {currentScore}";
        }
    }
}
