namespace Assets.Scripts.UI.Buttons.Actions
{
    public class QuitGameAction : IButtonAction
    {
        private readonly GameManager gameManager;

        public QuitGameAction(GameManager gameManager)
        {
            this.gameManager = gameManager;
        }

        public void Execute()
        {
            gameManager.QuitGame();
        }
    }
}