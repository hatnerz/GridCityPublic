using Assets.Scripts.UI.Buttons.Actions;
using Assets.Scripts.UI.Buttons;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Dtos;

namespace Assets.Scripts.UI.OverlayMenus
{
    internal class EndGameMenuManager : MonoBehaviour
    {
        [SerializeField] private FinalScoresInformation _playersScoresInformation;
        [SerializeField] private ActionButton _goToMenuButton;

        private void Start()
        {
            InitializeButtons();
        }

        public void SetFinalScoresInformation(string winner, List<PlayerScoreDisplayDto> playerScores)
        {
            _playersScoresInformation.SetPlayersScores(winner, playerScores);
        }

        private void InitializeButtons()
        {
            if (_goToMenuButton != null)
            {
                _goToMenuButton.SetAction(new BackToMenuAction(GameManager.Instance));
            }
            else
            {
                Debug.LogError("Go To Menu Button is not assigned in the inspector");
            }
        }
    }
}
