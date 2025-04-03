using GridCity.GameLogic.CellElements.BaseElements;

namespace GridCityServer.Dtos;

public record MoveResultDto(
    Guid PlacedPlayerId,
    Guid NextTurnPlayerId,
    BuildingType PlacedBuilding,
    int GridPositionX,
    int GridPositionY);
