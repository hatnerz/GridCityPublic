namespace GridCityServer.Dtos;

public record GameSessionPlayerDto(
    Guid GameSessionId,
    Guid PlayerId);
