using GridCity.GameLogic.Cards;

namespace Assets.Scripts.Dtos
{
    public class GameSessionMoveDto
    {
        public CardType PlayedCard { get; set; }
        public int GridPositionX { get; set; }
        public int GridPositionY { get; set; }
    }
}
