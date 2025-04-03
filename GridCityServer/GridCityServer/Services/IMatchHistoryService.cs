using GridCityServer.Dtos;

namespace GridCityServer.Services;
public interface IMatchHistoryService
{
    Task<PlayerHistoricalStatsDto> GetPlayerHistoricalStatsAsync(Guid playerId);
    Task SaveMatchResultAsync(GameSessionFinalResultDto finalResultDto);
}