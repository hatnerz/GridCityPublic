using UnityEngine;

namespace Assets.Scripts.UI.Buttons.Actions
{
    public class OpenPauseMenuAction : IButtonAction
    {
        private readonly GameObject pauseMenuParent;
        private readonly GameObject pauseMenuPrefab;

        public OpenPauseMenuAction(GameObject pauseMenuParent, GameObject pauseMenuPrefab)
        {
            this.pauseMenuParent = pauseMenuParent;
            this.pauseMenuPrefab = pauseMenuPrefab;
        }

        public void Execute()
        {
            Object.Instantiate(pauseMenuPrefab, pauseMenuParent.transform);
        }
    }
}
