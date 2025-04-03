namespace Assets.Scripts.UI.Buttons.Actions
{
    public class StartGameAction : IButtonAction
    {
        private readonly GameManager gameManager;

        public StartGameAction(GameManager gameManager)
        {
            this.gameManager = gameManager;
        }

        public void Execute()
        {
            gameManager.OpenLevelSelect();
        }
    }
}