using GridCity.GameLogic.Cards;

namespace GridCityServer.Dtos;

public record GameSessionMoveRequestDto
{
    public CardType PlayedCard { get; init; }
    public int GridPositionX { get; init; }
    public int GridPositionY { get; init; }
}
