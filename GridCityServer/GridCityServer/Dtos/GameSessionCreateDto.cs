namespace GridCityServer.Dtos;

public record GameSessionCreateDto(
    int GridSizeX,
    int GridSizeY,
    Guid LobbyId,
    List<Guid> PlayerIds);
