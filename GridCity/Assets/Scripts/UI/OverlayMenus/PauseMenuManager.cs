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
    public class PauseMenuManager : MonoBehaviour
    {
        [SerializeField] private ActionButton resumeButton;
        [SerializeField] private ActionButton goToMenuButton;
        [SerializeField] private GameObject pauseMenuObject;

        private void Start()
        {
            InitializeButtons();
        }
        private void InitializeButtons()
        {
            if (resumeButton != null)
            {
                resumeButton.SetAction(new ClosePauseMenuAction(pauseMenuObject));
            }
            else
            {
                Debug.LogError("Resume Button is not assigned in the inspector");
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
