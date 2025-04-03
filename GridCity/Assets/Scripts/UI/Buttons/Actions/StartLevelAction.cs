namespace Assets.Scripts.UI.Buttons.Actions
{
    public class StartLevelAction : IButtonAction
    {
        private readonly GameManager gameManager;

        public int LevelNumber { get; private set; }

        public StartLevelAction(int levelNumber, GameManager gameManager)
        {
            LevelNumber = levelNumber;
            this.gameManager = gameManager;
        }

        public void Execute()
        {
            gameManager.StartLevel(LevelNumber);
        }
    }
}