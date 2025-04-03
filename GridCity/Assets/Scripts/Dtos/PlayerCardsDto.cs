using GridCity.GameLogic.Cards;
using System.Collections.Generic;

namespace Assets.Scripts.Dtos
{
    public class PlayerCardsDto
    {
        public int CardsLeftInDeck { get; set; }
        public List<CardType> CardsInHand { get; set; }
    }
}
