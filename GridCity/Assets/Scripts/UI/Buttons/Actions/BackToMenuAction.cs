namespace Assets.Scripts.UI.Buttons.Actions
{
    public class BackToMenuAction : IButtonAction
    {
        private readonly GameManager gameManager;

        public BackToMenuAction(GameManager gameManager)
        {
            this.gameManager = gameManager;
        }

        public void Execute()
        {
            gameManager.ReturnToMainMenu();
        }
    }
}
