using GridCity.GameLogic.Cards;

namespace GridCityServer.Dtos;

public record PlayerCardsDto(
    int CardsLeftInDeck,
    List<CardType> CardsInHand);
