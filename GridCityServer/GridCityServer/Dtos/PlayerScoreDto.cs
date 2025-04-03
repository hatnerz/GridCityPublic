namespace GridCityServer.Dtos;

public record PlayerScoreDto(
    Guid PlayerId,
    int Score);
