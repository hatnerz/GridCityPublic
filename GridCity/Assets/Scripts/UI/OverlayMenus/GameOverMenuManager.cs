using Assets.Scripts.Core;
using Assets.Scripts.Gameplay;
using Assets.Scripts.Score;
using Assets.Scripts.UI.Buttons;
using Assets.Scripts.UI.Buttons.Actions;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

namespace Assets.Scripts.UI.Hud
{
    public class GameOverMenuManager : MonoBehaviour
    {
        [SerializeField] private ScoreInformation scoreInformation;
        [SerializeField] private ActionButton retryLevelButton;
        [SerializeField] private ActionButton goToMenuButton;

        private void Start()
        {
            InitializeButtons();
        }

        public void SetScoreInformation(int totalScore, int requiredScore)
        {
            scoreInformation.SetScoreInformation(totalScore, requiredScore);
        }

        private void InitializeButtons()
        {
            if (retryLevelButton != null)
            {
                var currentLevelNumber = GameState.CurrentLevelNumber;

                retryLevelButton.SetAction(new StartLevelAction(currentLevelNumber, GameManager.Instance));
            }
            else
            {
                Debug.LogError("Retry Level Button is not assigned in the inspector");
            }

            if (goToMenuButton != null)
            {
                goToMenuButton.SetAction(new BackToMenuAction(GameManager.Instance));
            }
            else
            {
                Debug.LogError("Go To Menu Button is not assigned in the inspector");
            }
        }
    }
}
