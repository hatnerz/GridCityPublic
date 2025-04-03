using Assets.Scripts.UI.Buttons;
using Assets.Scripts.UI.Buttons.Actions;
using UnityEngine;

namespace Assets.Scripts.UI.Menus
{
    public class MainMenuManager : MonoBehaviour
    {
        [SerializeField] private ActionButton startGameButton;
        [SerializeField] private ActionButton quitGameButton;
        [SerializeField] private ActionButton multiplayerButton;

        private void Start()
        {
            InitializeButtons();
        }

        private void InitializeButtons()
        {
            if (startGameButton != null)
            {
                startGameButton.SetAction(new StartGameAction(GameManager.Instance));
            }
            else
            {
                Debug.LogError("Start Game Button is not assigned in the inspector");
            }

            if (quitGameButton != null)
            {
                quitGameButton.SetAction(new QuitGameAction(GameManager.Instance));
            }
            else
            {
                Debug.LogError("Quit Game Button is not assigned in the inspector");
            }

            if (multiplayerButton != null)
            {
                multiplayerButton.SetAction(new OpenMultiplayerAction(GameManager.Instance));
            }
            else
            {
                Debug.LogError("Multiplayer Button is not assigned in the inspector");
            }
        }
    }
}