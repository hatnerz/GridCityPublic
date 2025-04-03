using GridCity.GameLogic.Cards;

namespace GridCityServer.Dtos;

public record GameSessionInitialDto(
    Guid Id,
    int GridSizeX,
    int GridSizeY,
    List<PlayerDto> Players,
    Guid FirstTurnPlayer,
    int InitialDeckSize);