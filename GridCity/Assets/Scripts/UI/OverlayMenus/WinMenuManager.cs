using Assets.Scripts.Core;
using Assets.Scripts.UI.Buttons;
using Assets.Scripts.UI.Buttons.Actions;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

namespace Assets.Scripts.UI.Hud
{
    public class WinMenuManager : MonoBehaviour
    {
        [SerializeField] private ScoreInformation scoreInformation;
        [SerializeField] private ActionButton nextLevelButton;
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
            if (nextLevelButton != null)
            {
                var levels = ResourceManager.Instance.LevelDataDictionary;
                var nextLevel = levels.GetValueOrDefault(GameState.CurrentLevelNumber + 1);
                if(nextLevel != null)
                {
                    nextLevelButton.SetAction(new StartLevelAction(nextLevel.LevelNumber, GameManager.Instance));
                }
                else
                {
                    nextLevelButton.gameObject.SetActive(false);
                }
            }
            else
            {
                Debug.LogError("Next Level Button is not assigned in the inspector");
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
