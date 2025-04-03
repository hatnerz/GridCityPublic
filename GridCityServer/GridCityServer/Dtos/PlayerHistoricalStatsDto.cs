namespace GridCityServer.Dtos;

public record PlayerHistoricalStatsDto(
    Guid PlayerId,
    int TotalMatches,
    int WinMatches);
