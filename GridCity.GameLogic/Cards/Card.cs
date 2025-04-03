namespace GridCity.GameLogic.Cards
{
    public class Card
    {
        public Card(CardCategory category, CardType type)
        {
            Category = category;
            Type = type;
            Played = false;
        }

        public CardCategory Category { get; private set; }
        public CardType Type { get; private set; }

        public bool Played { get; private set; }
    }
}