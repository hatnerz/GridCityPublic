using GridCityServer.Database;
using GridCityServer.Dtos;
using GridCityServer.Models;
using Microsoft.EntityFrameworkCore;

namespace GridCityServer.Services;

public class MatchHistoryService : IMatchHistoryService
{
    private readonly ApplicationDbContext _dbContext;

    public MatchHistoryService(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task SaveMatchResultAsync(GameSessionFinalResultDto finalResultDto)
    {
        var matchPlayers = finalResultDto.PlayerScores.Select(e => new MatchPlayer(e.PlayerId, e.Score)).ToList();
        var matchResult = new MatchHistory(matchPlayers, finalResultDto.Winner.PlayerId);
        _dbContext.MatchesHistory.Add(matchResult);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<PlayerHistoricalStatsDto> GetPlayerHistoricalStatsAsync(Guid playerId)
    {
        var totalMatches = await _dbContext.MatchPlayers.Where(e => e.PlayerId == playerId).CountAsync();
        var winMatches = await _dbContext.MatchesHistory.Where(e => e.WinnerPlayerId == playerId).CountAsync();
        return new(playerId, totalMatches, winMatches);
    }
}
