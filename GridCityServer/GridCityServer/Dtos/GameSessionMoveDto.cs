using GridCity.GameLogic.Cards;

namespace GridCityServer.Dtos;

public record GameSessionMoveDto(
    Guid PlayerId,
    Guid GameSessionId,
    int GridPositionX,
    int GridPositionY,
    CardType Card);