namespace GridCityServer.Dtos;

public record GameSessionFinalResultDto(
    List<PlayerScoreDto> PlayerScores,
    PlayerScoreDto Winner,
    TimeSpan Duration);