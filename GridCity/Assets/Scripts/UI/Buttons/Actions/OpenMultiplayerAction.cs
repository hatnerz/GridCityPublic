namespace Assets.Scripts.UI.Buttons.Actions
{
    public class OpenMultiplayerAction : IButtonAction
    {
        private readonly GameManager gameManager;

        public OpenMultiplayerAction(GameManager gameManager)
        {
            this.gameManager = gameManager;
        }

        public void Execute()
        {
            gameManager.OpenMultiplayer();
        }
    }
}
