namespace GridCityServer.Dtos;

public record LobbyPlayerDto(
    Guid PlayerId,
    Guid LobbyId);
