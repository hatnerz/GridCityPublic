using UnityEngine;

namespace Assets.Scripts.UI.Buttons.Actions
{
    public class ClosePauseMenuAction : IButtonAction
    {
        private readonly GameObject pauseMenuObject;

        public ClosePauseMenuAction(GameObject pauseMenuObject)
        {
            this.pauseMenuObject = pauseMenuObject;
        }

        public void Execute()
        {
            Object.Destroy(pauseMenuObject);
        }
    }
}
